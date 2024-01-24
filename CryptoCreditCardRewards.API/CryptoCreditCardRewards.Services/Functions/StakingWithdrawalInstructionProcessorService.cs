using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Functions.Interfaces;
using CryptoCreditCardRewards.Utilities;
using Microsoft.Extensions.Logging;
using CryptoCreditCardRewards.Models.Exceptions;

namespace CryptoCreditCardRewards.Services.Functions
{
    public class StakingWithdrawalInstructionProcessorService : IStakingWithdrawalInstructionProcessorService
    {
        private readonly IInstructionService _instructionService;
        private readonly ITransactionService _transactionService;
        private readonly IWalletAddressService _walletAddressService;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly IBlockchainProviderFactory _blockchainServiceProviderFactory;
        private readonly ISystemWalletAddressService _systemWalletAddressService;
        private readonly ILogger<StakingWithdrawalInstructionProcessorService> _logger;
        private readonly SystemWalletAddressSettings _systemWalletSettings;
        private readonly WalletAddressSettings _walletAddressSettings;

        public StakingWithdrawalInstructionProcessorService(IInstructionService instructionService, ITransactionService transactionService, IWalletAddressService walletAddressService,
            ICryptoCurrencyService cryptoCurrencyService, IBlockchainProviderFactory blockchainServiceProviderFactory, ISystemWalletAddressService systemWalletAddressService, IOptions<SystemWalletAddressSettings> settings,
            IOptions<WalletAddressSettings> walletAddressSettings, ILogger<StakingWithdrawalInstructionProcessorService> logger)
        {
            _instructionService = instructionService;
            _transactionService = transactionService;
            _blockchainServiceProviderFactory = blockchainServiceProviderFactory;
            _walletAddressService = walletAddressService;
            _cryptoCurrencyService = cryptoCurrencyService;
            _systemWalletAddressService = systemWalletAddressService;
            _systemWalletSettings = settings.Value;
            _walletAddressSettings = walletAddressSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Process staging withdrawal instructions (try to move from staking hot wallet to users wallet).
        /// </summary>
        /// <returns>An async task</returns>
        public async Task ProcessStakingWithdrawalInstructionsAsync()
        {
            // Get all instructions to pay
            var paymentInstructionsToProcess = _instructionService.GetStakingWithdrawalInstructionsToProcess();
            if (paymentInstructionsToProcess != null)
            {
                // Complete hte instructions
                foreach (var paymentInstructionToProcess in paymentInstructionsToProcess)
                {
                    // Process the payment instruction
                    await CompleteStakingForWithdrawalInstructionAsync(paymentInstructionToProcess.Id);
                }
            }
        }

        #region Helpers

        /// <summary>
        /// Process a withdrawal instruction (stake the deposited value) by its ID
        /// </summary>
        /// <param name="paymentInstructionId">The payment instruction to complete</param>
        /// <returns>The staked transaction</returns>
        private async Task<Transaction?> CompleteStakingForWithdrawalInstructionAsync(int paymentInstructionId)
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
                var systemWalletToUse = _systemWalletAddressService.GetSystemWalletAddress(cryptoCurrency.Id, AddressType.StakingDeposit, ActiveState.Active);

                // Get the users balance
                var balance = await blockChainService.GetBalanceAsync(systemWalletToUse.Address);
                var fee = await blockChainService.GetEstimatedTransactionPriceAsync(systemWalletToUse.Address, blockChainService.GetPrivateKey(systemWalletToUse.KeyData, _systemWalletSettings.Password), -1*paymentInstruction.Amount);
                if (balance < ((-1* paymentInstruction.Amount) + fee.MonetaryFee))
                {
                    // Put back the instruction
                    await _instructionService.PutBackInstructionToProcessLaterAsync(paymentInstruction.Id);

                    return null;
                }

                // Make payment
                var hash = await blockChainService.SendTransactionAsync(systemWalletToUse.Address, blockChainService.GetPrivateKey(systemWalletToUse.KeyData, _systemWalletSettings.Password), walletAddress.Address,
                    paymentInstruction.Amount*-1, fee.MakeTransactionFee);

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
                return await _transactionService.CreateTransactionAsync(TransactionType.Staking, paymentInstructionId, cryptoCurrency.Id, walletAddress.Id, systemWalletToUse.Id, paymentInstruction.WhitelistAddressId,
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
