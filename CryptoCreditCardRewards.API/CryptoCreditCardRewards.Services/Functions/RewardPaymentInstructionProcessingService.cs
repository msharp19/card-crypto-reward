using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Functions.Interfaces;
using CryptoCreditCardRewards.Utilities;

namespace CryptoCreditCardRewards.Services.Functions
{
    public class RewardPaymentInstructionProcessingService : IRewardPaymentInstructionProcessingService
    {
        private readonly IInstructionService _instructionService;
        private readonly IBlockchainProviderFactory _blockchainServiceProviderFactory;
        private readonly IWalletAddressService _walletAddressService;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly ITransactionService _transactionService;
        private readonly ISystemWalletAddressService _systemWalletAddressService;
        private readonly ILogger<RewardPaymentInstructionProcessingService> _logger;
        private readonly SystemWalletAddressSettings _settings;

        public RewardPaymentInstructionProcessingService(IInstructionService instructionService, IBlockchainProviderFactory blockchainServiceProviderFactory,
            IWalletAddressService walletAddressService, ICryptoCurrencyService cryptoCurrencyService, ITransactionService transactionService, ISystemWalletAddressService systemWalletAddressService,
            IOptions<SystemWalletAddressSettings> settings, ILogger<RewardPaymentInstructionProcessingService> logger)
        {
            _instructionService = instructionService;
            _blockchainServiceProviderFactory = blockchainServiceProviderFactory;
            _walletAddressService = walletAddressService;
            _transactionService = transactionService;
            _cryptoCurrencyService = cryptoCurrencyService;
            _systemWalletAddressService = systemWalletAddressService;
            _logger = logger;
            _settings = settings.Value;
        }

        /// <summary>
        /// Processes all reward payment instructions of type "RewardPayment".
        /// These are reward payments to users based on their spend/reward selection
        /// </summary>
        /// <returns>An async task</returns>
        public async Task ProcessRewardPaymentInstructionsAsync()
        {
            // Get all instructions to pay
            var paymentInstructionsToProcess = _instructionService.GetRewardPaymentInstructionsToProcess();
            if (paymentInstructionsToProcess != null)
            {
                // Complete hte instructions
                foreach (var paymentInstructionToProcess in paymentInstructionsToProcess)
                {
                    // Process the payment instruction
                    await CompleteRewardPaymentForInstructionAsync(paymentInstructionToProcess.Id);
                }
            }
        }

        #region Helpers

        /// <summary>
        /// Process a reward payment instruction by its ID (complete the payment)
        /// </summary>
        /// <param name="paymentInstructionId">The payment instruction to complete</param>
        /// <returns></returns>
        private async Task<Transaction?> CompleteRewardPaymentForInstructionAsync(int paymentInstructionId)
        {
            // Get instruction
            var paymentInstruction = await _instructionService.PickupInstructionToProcessAsync(paymentInstructionId);

            try
            {
                // Get the wallet address
                var walletAddress = _walletAddressService.GetWalletAddressById(paymentInstruction.WalletAddressId.Value);

                // We honor the crypto currency payment so even if inactive, is returned
                var cryptoCurrency = _cryptoCurrencyService.GetCryptoCurrency(walletAddress.CryptoCurrencyId, ActiveState.Both);
                if (!cryptoCurrency.Active)
                {
                    _logger.LogInformation($"Cryptocurrency with ID: {cryptoCurrency.Id} is inactive. Instruction {paymentInstructionId} could not be processed");

                    throw new ForbidException(FailedReason.CryptoCurrencyIsCurrentlyInactive);
                }

                // Get the blockchain service
                var blockChainService = _blockchainServiceProviderFactory.GetBlockchainService(cryptoCurrency.InfrastructureType, cryptoCurrency.IsTestNetwork ? NetworkType.Test : NetworkType.Main,
                    cryptoCurrency.NetworkEndpoint);

                // Get the system wallet the payment is associated with
                var systemWalletToUse = _systemWalletAddressService.GetSystemWalletAddress(cryptoCurrency.Id, AddressType.RewardIssuance, ActiveState.Active);
                var systemBalance = await blockChainService.GetBalanceAsync(systemWalletToUse.Address);
                var fee = await blockChainService.GetEstimatedTransactionPriceAsync(systemWalletToUse.Address, blockChainService.GetPrivateKey(systemWalletToUse.KeyData, _settings.Password), paymentInstruction.Amount);
                if (systemBalance < (paymentInstruction.Amount + fee.MonetaryFee))
                {
                    await _instructionService.PutBackInstructionToProcessLaterAsync(paymentInstructionId);

                    // Log issue with wallet balance
                    _logger.LogCritical($"RewardPaymentInstructionProcessingService: Not enough fees to cover {fee} + {paymentInstruction.Amount} in account {systemWalletToUse.Address} for id {systemWalletToUse.Id}");

                    return null;
                }

                // Make the payment
                var hash = await blockChainService.SendTransactionAsync(systemWalletToUse.Address, blockChainService.GetPrivateKey(systemWalletToUse.KeyData, _settings.Password),
                    walletAddress.Address, paymentInstruction.Amount, fee.MakeTransactionFee);

                // Transaction failed so we mark instruction as failed
                if (string.IsNullOrEmpty(hash))
                {
                    // Mark the instruction as failed
                    await _instructionService.FailInstructionAsync(paymentInstruction.Id, "Transaction has not generated");

                    return null;
                }
                else
                {
                    // Complete instruction
                    await _instructionService.CompleteInstructionAsync(paymentInstruction.Id);
                }

                // Create a record of transaction our side
                return await _transactionService.CreateTransactionAsync(TransactionType.Reward, paymentInstructionId, cryptoCurrency.Id, walletAddress.Id, systemWalletToUse.Id, null,
                    systemWalletToUse.Address, walletAddress.Address, paymentInstruction.UserId, hash, paymentInstruction.Amount, true);
            }
            catch (Exception ex)
            {
                // Put back instruction
                await _instructionService.PutBackInstructionToProcessLaterAsync(paymentInstructionId);

                // Log critical error
                _logger.LogCritical(ex.StackTrace);

                return null;
            }
        }

        #endregion
    }
}
