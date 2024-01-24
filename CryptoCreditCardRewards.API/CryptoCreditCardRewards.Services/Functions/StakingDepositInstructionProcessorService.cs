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

namespace CryptoCreditCardRewards.Services.Functions
{
    public class StakingDepositInstructionProcessorService : IStakingDepositInstructionProcessorService
    {
        private readonly IInstructionService _instructionService;
        private readonly ITransactionService _transactionService;
        private readonly IWalletAddressService _walletAddressService;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly IBlockchainProviderFactory _blockchainServiceProviderFactory;
        private readonly ISystemWalletAddressService _systemWalletAddressService;
        private readonly ILogger<StakingDepositInstructionProcessorService> _logger;
        private readonly StakingSettings _stakingSettings;
        private readonly WalletAddressSettings _walletAddressSettings;

        public StakingDepositInstructionProcessorService(IInstructionService instructionService, ITransactionService transactionService, IWalletAddressService walletAddressService,
            ICryptoCurrencyService cryptoCurrencyService, IBlockchainProviderFactory blockchainServiceProviderFactory, ISystemWalletAddressService systemWalletAddressService,
            IOptions<StakingSettings> settings, IOptions<WalletAddressSettings> walletAddressSettings, ILogger<StakingDepositInstructionProcessorService> logger)
        {
            _instructionService = instructionService;
            _transactionService = transactionService;
            _systemWalletAddressService = systemWalletAddressService;
            _logger = logger;
            _blockchainServiceProviderFactory = blockchainServiceProviderFactory;
            _walletAddressService = walletAddressService;
            _cryptoCurrencyService = cryptoCurrencyService;
            _stakingSettings = settings.Value;
            _walletAddressSettings = walletAddressSettings.Value;
        }

        /// <summary>
        /// Process staging deposit instructions (try to taker deposit and move to staking hot wallet).
        /// </summary>
        /// <returns>An async task</returns>
        public async Task ProcessStakingDepositInstructionsAsync()
        {
            // Get all instructions to pay
            var paymentInstructionsToProcess = _instructionService.GetStakingDepositInstructionsToProcess();
            if (paymentInstructionsToProcess != null)
            {
                // Complete hte instructions
                foreach (var paymentInstructionToProcess in paymentInstructionsToProcess)
                {
                    // Process the payment instruction
                    await CompleteStakingForDepositInstructionAsync(paymentInstructionToProcess.Id);
                }
            }
        }

        #region Helpers

        /// <summary>
        /// Process a deposit instruction (stake the deposited value) by its ID
        /// </summary>
        /// <param name="paymentInstructionId">The payment instruction to complete</param>
        /// <returns>The staked transaction</returns>
        private async Task<Transaction?> CompleteStakingForDepositInstructionAsync(int paymentInstructionId)
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

                // Get the users balance
                var balance = await blockChainService.GetBalanceAsync(walletAddress.Address);
                var fee = await blockChainService.GetEstimatedTransactionPriceAsync(walletAddress.Address, blockChainService.GetPrivateKey(walletAddress.KeyData, _walletAddressSettings.Password), paymentInstruction.Amount);
                if (balance < (paymentInstruction.Amount + fee.MonetaryFee))
                {
                    // Put back the instruction
                    await _instructionService.PutBackInstructionToProcessLaterAsync(paymentInstruction.Id);

                    // Log issue with wallet balance
                    _logger.LogCritical($"StakingDepositInstructionProcessorService: Not enough fees to cover {fee} + {paymentInstruction.Amount} in account {walletAddress.Address} for id {walletAddress.Id}");

                    return null;
                }

                // Get the system wallet the payment is associated with
                var systemWalletToUse = _systemWalletAddressService.GetSystemWalletAddress(cryptoCurrency.Id, AddressType.StakingDeposit, ActiveState.Active);

                // Create transaction
                var hash = await blockChainService.SendTransactionAsync(walletAddress.Address, blockChainService.GetPrivateKey(walletAddress.KeyData, _walletAddressSettings.Password), systemWalletToUse.Address,
                    paymentInstruction.Amount, fee.MakeTransactionFee);

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

                // Create a fee transaction
                await _transactionService.CreateTransactionAsync(TransactionType.Fee, paymentInstructionId, cryptoCurrency.Id, walletAddress.Id, systemWalletToUse.Id, null, walletAddress.Address, systemWalletToUse.Address,
                    paymentInstruction.UserId, hash, paymentInstruction.MonetaryFee * -1, true);

                // Create a record of transaction our side
                return await _transactionService.CreateTransactionAsync(TransactionType.Staking, paymentInstructionId, cryptoCurrency.Id, walletAddress.Id, systemWalletToUse.Id, null, walletAddress.Address,
                    systemWalletToUse.Address, paymentInstruction.UserId, hash, paymentInstruction.Amount, true);
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
