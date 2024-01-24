using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Functions.Interfaces;
using CryptoCreditCardRewards.Services.Http.Interfaces;

namespace CryptoCreditCardRewards.Services.Functions
{
    public class MonthlyRewardInstructionIssuerService : IMonthlyRewardInstructionIssuerService
    {
        private readonly IInstructionService _instructionService;
        private readonly IUserRewardSelectionService _userRewardSelectionService;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly IConversionProviderFactory _conversionProviderFactory;
        private readonly IWalletAddressService _walletAddressService;
        private readonly ISystemWalletAddressService _systemWalletAddressService;
        private readonly IBlockchainProviderFactory _blockchainProviderFactory;
        private readonly ILogger<MonthlyRewardInstructionIssuerService> _logger;
        private readonly SystemWalletAddressSettings _settings;

        public MonthlyRewardInstructionIssuerService(IInstructionService instructionService, IUserRewardSelectionService userRewardSelectionService, ICryptoCurrencyService cryptoCurrencyService,
            IConversionProviderFactory conversionProviderFactory, IWalletAddressService walletAddressService, ISystemWalletAddressService systemWalletAddressService, IBlockchainProviderFactory blockchainProviderFactory,
            IOptions<SystemWalletAddressSettings> settings, ILogger<MonthlyRewardInstructionIssuerService> logger)
        {
            _instructionService = instructionService;
            _userRewardSelectionService = userRewardSelectionService;
            _cryptoCurrencyService = cryptoCurrencyService;
            _conversionProviderFactory = conversionProviderFactory;
            _walletAddressService = walletAddressService;
            _systemWalletAddressService = systemWalletAddressService;
            _blockchainProviderFactory = blockchainProviderFactory;
            _settings = settings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Processes all monthly rewards into payment instructions of type "RewardPayment".
        /// These are reward payments to users based on their spend/reward selection
        /// </summary>
        /// <returns>An async task</returns>
        public async Task ProcessMonthlyRewardInstructionsAsync()
        {
            // Get all instructions to pay
            var paymentInstructionsToProcess = _instructionService.GetMonthlyRewardInstructionsToProcess();
            if (paymentInstructionsToProcess != null)
            {
                // Complete hte instructions
                foreach (var paymentInstructionToProcess in paymentInstructionsToProcess)
                {
                    // Process the payment instruction
                    await CreatePaymentInstructionsAsync(paymentInstructionToProcess.Id);
                }
            }
        }

        #region Helpers

        /// <summary>
        /// Create a set of payment instructions from a parent monthly reward instruction
        /// </summary>
        /// <param name="paymentInstructionId">The parent to create the children from</param>
        /// <returns>The new instructions created</returns>
        private async Task<List<Instruction>> CreatePaymentInstructionsAsync(int paymentInstructionId)
        {
            // Get parent instruction
            var parentInstruction = await _instructionService.PickupInstructionToProcessAsync(paymentInstructionId);

            try
            {
                // Get selections for the user to reward
                var selections = _userRewardSelectionService.GetUserRewardSelections(parentInstruction.UserId);

                // Get all crypto currencies for selections
                var cryptoCurrencyIds = selections.Select(x => x.CryptoCurrencyId).ToList();
                var cryptoCurrencies = _cryptoCurrencyService.GetCryptoCurrenciesByIds(cryptoCurrencyIds);

                // Get all wallets for selections
                var wallets = _walletAddressService.GetWalletAddresses(parentInstruction.UserId, cryptoCurrencyIds);

                // Check theres a system wallet otherwise this will fail later
                var systemWallet = _systemWalletAddressService.GetSystemWalletAddresses(cryptoCurrencyIds, AddressType.RewardIssuance, ActiveState.Active).FirstOrDefault();

                // Create the payment instructions
                var paymentInstructionsToAdd = new List<Instruction>();
                foreach (var selection in selections)
                {
                    // Find the amount for the partition
                    var amount = (parentInstruction.Amount / 100m) * selection.ContributionPercentage;
                    if (amount > 0)
                    {
                        // Get the currency to convert to
                        var cryptoCurrency = cryptoCurrencies.FirstOrDefault(x => x.Id == selection.CryptoCurrencyId);

                        // Convert to currency
                        var conversionService = _conversionProviderFactory.GetConversionService(cryptoCurrency.ConversionServiceType);
                        var convertedAmount = await conversionService.ConvertAsync(amount, "HKD", cryptoCurrency.Symbol);

                        // Get users wallet address
                        var walletAddress = wallets.Where(x => x.CryptoCurrencyId == selection.CryptoCurrencyId)
                            .FirstOrDefault();

                        // Get the blockchain service
                        var blockChainService = _blockchainProviderFactory.GetBlockchainService(cryptoCurrency.InfrastructureType, cryptoCurrency.IsTestNetwork ? NetworkType.Test : NetworkType.Main,
                            cryptoCurrency.NetworkEndpoint);

                        // Get fees
                        var fees = await blockChainService.GetEstimatedTransactionPriceAsync(systemWallet.Address, blockChainService.GetPrivateKey(systemWallet.KeyData, _settings.Password), convertedAmount.Value);

                        var newInstruction = new Instruction(true, convertedAmount.Value, selection.UserId, walletAddress.Id, parentInstruction.Id, null, parentInstruction.FromDate, parentInstruction.ToDate,
                            convertedAmount.Rate, fees.MonetaryFee, fees.MakeTransactionFee);
                        newInstruction.SetType(InstructionType.RewardPayment);

                        // Create payment instruction
                        paymentInstructionsToAdd.Add(newInstruction);
                    }
                }

                // Set parent reward instruction as completed if payment instructions have been created
                if (selections.Any())
                    await _instructionService.CompleteInstructionAsync(parentInstruction.Id);
                else await _instructionService.PutBackInstructionToProcessLaterAsync(parentInstruction.Id);

                // Add instructions
                return await _instructionService.AddInstructionsAsync(paymentInstructionsToAdd);
            }
            catch (Exception ex)
            {
                // Put back instruction
                await _instructionService.PutBackInstructionToProcessLaterAsync(parentInstruction.Id);

                // Log critical error
                _logger.LogCritical(ex.StackTrace);

                return null;
            }
        }

        #endregion
    }
}
