using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Events;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Http.Interfaces;

namespace CryptoCreditCardRewards.Services.EventHandlers
{
    // DEPRECATED
    [Obsolete("Stopped doing this as an event and started doing this as a separate job")]
    public class CreatedRewardInstructionHandler : INotificationHandler<CreatedRewardInstructionEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CreatedRewardInstructionHandler> _logger;

        public CreatedRewardInstructionHandler(IServiceProvider serviceProvider, ILogger<CreatedRewardInstructionHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(CreatedRewardInstructionEvent notification, CancellationToken cancellationToken)
        {
            var instructionService = _serviceProvider.GetRequiredService<IInstructionService>();
            var userRewardSelectionService = _serviceProvider.GetRequiredService<IUserRewardSelectionService>();
            var cryptoCurrencyService = _serviceProvider.GetRequiredService<ICryptoCurrencyService>();
            var conversionServiceFactory = _serviceProvider.GetRequiredService<IConversionProviderFactory>();
            var walletAddressService = _serviceProvider.GetRequiredService<IWalletAddressService>();
            var systemWalletAddressService = _serviceProvider.GetRequiredService<ISystemWalletAddressService>();
            var blockchainServiceProviderFactory = _serviceProvider.GetRequiredService<IBlockchainProviderFactory>();
            var settings = _serviceProvider.GetRequiredService<IOptions<SystemWalletAddressSettings>>().Value;

            // Get parent instruction
            var parentInstruction = instructionService.GetInstruction(notification.Instruction.Id, ActiveState.Active); 

            try
            {
                // Get selections for the user to reward
                var selections = userRewardSelectionService.GetUserRewardSelections(parentInstruction.UserId);

                // Get all crypto currencies for selections
                var cryptoCurrencyIds = selections.Select(x => x.CryptoCurrencyId).ToList();
                var cryptoCurrencies = cryptoCurrencyService.GetCryptoCurrenciesByIds(cryptoCurrencyIds);

                // Get all wallets for selections
                var wallets = walletAddressService.GetWalletAddresses(parentInstruction.UserId, cryptoCurrencyIds);

                // Check theres a system wallet otherwise this will fail later
                var systemWallet = systemWalletAddressService.GetSystemWalletAddresses(cryptoCurrencyIds, AddressType.RewardIssuance, ActiveState.Active).FirstOrDefault();

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
                        var conversionService = conversionServiceFactory.GetConversionService(cryptoCurrency.ConversionServiceType);
                        var convertedAmount = await conversionService.ConvertAsync(amount, "HKD", cryptoCurrency.Symbol);

                        // Get users wallet address
                        var walletAddress = wallets.Where(x => x.CryptoCurrencyId == selection.CryptoCurrencyId)
                            .FirstOrDefault();

                        // Get the blockchain service
                        var blockChainService = blockchainServiceProviderFactory.GetBlockchainService(cryptoCurrency.InfrastructureType, cryptoCurrency.IsTestNetwork ? NetworkType.Test : NetworkType.Main,
                            cryptoCurrency.NetworkEndpoint);

                        // Get fees
                        var fees = await blockChainService.GetEstimatedTransactionPriceAsync(systemWallet.Address, blockChainService.GetPrivateKey(systemWallet.KeyData, settings.Password), convertedAmount.Value);

                        var newInstruction = new Instruction(true, convertedAmount.Value, selection.UserId, walletAddress.Id, parentInstruction.Id, null, parentInstruction.FromDate, parentInstruction.ToDate, 
                            convertedAmount.Rate, fees.MonetaryFee, fees.MakeTransactionFee);
                        newInstruction.SetType(InstructionType.RewardPayment);

                        // Create payment instruction
                        paymentInstructionsToAdd.Add(newInstruction);
                    }
                }

                // Set parent reward instruction as completed if payment instructions have been created
                if (selections.Any())
                    await instructionService.CompleteInstructionAsync(parentInstruction.Id);
                else await instructionService.PutBackInstructionToProcessLaterAsync(parentInstruction.Id);

                // Add instructions
                await instructionService.AddInstructionsAsync(paymentInstructionsToAdd);
            }
            catch (Exception ex)
            {
                // Put back instruction
                await instructionService.PutBackInstructionToProcessLaterAsync(parentInstruction.Id);

                // Log critical error
                _logger.LogCritical(ex.StackTrace);
            }

            return;
        }
    }
}
