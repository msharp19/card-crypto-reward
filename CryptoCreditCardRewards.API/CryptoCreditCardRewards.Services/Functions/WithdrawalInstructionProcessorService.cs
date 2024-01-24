using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Functions.Interfaces;
using CryptoCreditCardRewards.Models.Exceptions;
using Microsoft.Extensions.Logging;

namespace CryptoCreditCardRewards.Services.Functions
{
    public class WithdrawalInstructionProcessorService : IWithdrawalInstructionProcessorService
    {
        private readonly IInstructionService _instructionService;
        private readonly ITransactionService _transactionService;
        private readonly IWalletAddressService _walletAddressService;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly IBlockchainProviderFactory _blockchainServiceProviderFactory;
        private readonly ISystemWalletAddressService _systemWalletAddressService;
        private readonly IWhitelistAddressService _whitelistAddressService;
        private readonly ILogger<WithdrawalInstructionProcessorService> _logger;
        private readonly WalletAddressSettings _walletAddressSettings;

        public WithdrawalInstructionProcessorService(IInstructionService instructionService, ITransactionService transactionService, IWalletAddressService walletAddressService,
            ICryptoCurrencyService cryptoCurrencyService, IBlockchainProviderFactory blockchainServiceProviderFactory, ISystemWalletAddressService systemWalletAddressService,
            IWhitelistAddressService whitelistAddressService, IOptions<WalletAddressSettings> walletAddressSettings, ILogger<WithdrawalInstructionProcessorService> logger)
        {
            _instructionService = instructionService;
            _transactionService = transactionService;
            _blockchainServiceProviderFactory = blockchainServiceProviderFactory;
            _walletAddressService = walletAddressService;
            _cryptoCurrencyService = cryptoCurrencyService;
            _systemWalletAddressService = systemWalletAddressService;
            _whitelistAddressService = whitelistAddressService;
            _walletAddressSettings = walletAddressSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Process withdrawal instructions (try to move from staking hot wallet to users wallet).
        /// </summary>
        /// <returns>An async task</returns>
        public async Task ProcessWithdrawalInstructionsAsync()
        {
            // Get all instructions to pay
            var paymentInstructionsToProcess = _instructionService.GetWithdrawalInstructionsToProcess();
            if (paymentInstructionsToProcess != null)
            {
                // Complete the instructions
                foreach (var paymentInstructionToProcess in paymentInstructionsToProcess)
                {
                    // Process the payment instruction
                    await CompleteWithdrawalInstructionAsync(paymentInstructionToProcess.Id);
                }
            }
        }

        #region Helpers

        /// <summary>
        /// Process a withdrawal instruction
        /// </summary>
        /// <param name="paymentInstructionId">The payment instruction to complete</param>
        /// <returns>The staked transaction</returns>
        private async Task<Transaction?> CompleteWithdrawalInstructionAsync(int paymentInstructionId)
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
                var fee = await blockChainService.GetEstimatedTransactionPriceAsync(walletAddress.Address, blockChainService.GetPrivateKey(walletAddress.KeyData, _walletAddressSettings.Password), -1*paymentInstruction.Amount);
                if (balance < (paymentInstruction.Amount + fee.MonetaryFee))
                {
                    // Put back the instruction
                    await _instructionService.PutBackInstructionToProcessLaterAsync(paymentInstruction.Id);

                    return null;
                }

                // Get the address to send to
                var whitelistAddress = _whitelistAddressService.GetWhitelistAddress(paymentInstruction.WhitelistAddressId ?? -1, WhitelistAddressState.Vaild);
                if (whitelistAddress == null)
                    throw new ForbidException(FailedReason.WhitelistAddressNotValid);

                // Make the payment
                var hash = await blockChainService.SendTransactionAsync(walletAddress.Address, blockChainService.GetPrivateKey(walletAddress.KeyData, _walletAddressSettings.Password), whitelistAddress.Address,
                    -1*paymentInstruction.Amount, fee.MakeTransactionFee);

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
                return await _transactionService.CreateTransactionAsync(TransactionType.Withdrawal, paymentInstructionId, cryptoCurrency.Id, walletAddress.Id, null, paymentInstruction.WhitelistAddressId, walletAddress.Address,
                    walletAddress.Address, paymentInstruction.UserId, hash, paymentInstruction.Amount, true);
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
