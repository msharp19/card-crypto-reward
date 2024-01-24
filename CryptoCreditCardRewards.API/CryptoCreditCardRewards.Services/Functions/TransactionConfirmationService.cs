using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Functions.Interfaces;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;

namespace CryptoCreditCardRewards.Services.Functions
{
    public class TransactionConfirmationService : ITransactionConfirmationService
    {
        private readonly ITransactionService _transactionService;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly IBlockchainProviderFactory _blockchainServiceProviderFactory;

        public TransactionConfirmationService(ITransactionService transactionService, ICryptoCurrencyService cryptoCurrencyService, IBlockchainProviderFactory blockchainServiceProviderFactory)
        {
            _transactionService = transactionService;
            _cryptoCurrencyService = cryptoCurrencyService;
            _blockchainServiceProviderFactory = blockchainServiceProviderFactory;
        }

        /// <summary>
        /// Try to confirm any unconfirmed transactions
        /// </summary>
        /// <returns>An async task</returns>
        public async Task ConfirmUnConfirmedTransactionsAsync()
        {
            // Get all unconfirmed transactions
            var unconfirmedTransactions = _transactionService.GetUnconfirmedTransactions();
            if (unconfirmedTransactions != null)
            {
                // Try to confirm the transaction
                foreach (var unconfirmedTransaction in unconfirmedTransactions)
                {
                    await TryToConfirmTransactionAsync(unconfirmedTransaction);
                }
            }
        }

        #region Helpers

        /// <summary>
        /// Process an unconfirmed transaction.
        /// This looks for the transaction hash on the respective blockchain, if found it is considered confirmed
        /// </summary>
        /// <param name="transactionId">The transaction to confirm</param>
        /// <returns>The transaction if confirmed</returns>
        private async Task<Transaction?> TryToConfirmTransactionAsync(Transaction transaction)
        {
            // Get cryptocurrency for transaction
            var cryptoCurrency = _cryptoCurrencyService.GetCryptoCurrency(transaction.CryptoCurrencyId);

            // Get the blockchain service
            var blockChainService = _blockchainServiceProviderFactory.GetBlockchainService(cryptoCurrency.InfrastructureType, cryptoCurrency.IsTestNetwork ? NetworkType.Test : NetworkType.Main,
                cryptoCurrency.NetworkEndpoint);

            // Get the transactions state
            var transactionState = await blockChainService.GetTransactionStateAsync(transaction.Hash);

            // If finished, mark the state in the transaction
            if (transactionState == TransactionState.Completed || transactionState == TransactionState.Failed)
            {
                // Confirm
                return await _transactionService.ConfirmTransactionAsync(transaction.Id, transactionState);
            }

            // If is pending, we do nothing
            return null;
        }

        #endregion
    }
}
