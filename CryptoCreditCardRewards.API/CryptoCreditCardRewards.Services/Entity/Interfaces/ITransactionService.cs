using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos.CreditCardTransactions;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Misc;

namespace CryptoCreditCardRewards.Services.Entity.Interfaces
{
    public interface ITransactionService
    {

        /// <summary>
        /// Get the aggregate transaction value
        /// </summary>
        /// <param name="fromDate">The date the aggregate value is from</param>
        /// <param name="toDate">The date the aggregate value is to</param>
        /// <param name="userId">The user the aggregate transaction value is for</param>
        /// <returns>An aggregate transaction value over a period for a user</returns>
        Task<List<AggregateTransactionValueDto>> GetUserAggregateStakingTransactionValueAsync(DateTime fromDate, DateTime toDate, int userId);

        /// <summary>
        /// Get a list of unconfirmed transactions
        /// </summary>
        /// <returns>Unconfirmed transaction</returns>
        List<Transaction> GetUnconfirmedTransactions();

        /// <summary>
        /// Review a transaction
        /// </summary>
        /// <param name="transactionId">The transaction</param>
        /// <param name="failed">If the review failed</param>
        /// <param name="failedReason">The reason for failure (if any)</param>
        /// <returns>The reviewed transaction</returns>
        Task<Transaction> ReviewTransactionAsync(int transactionId, bool failed, string? failedReason);

        /// <summary>
        /// Get transactions paged  for user 
        /// </summary>
        /// <param name="userId">The user</param>
        /// <param name="search">A search term (crypto name & symbol)</param>
        /// <param name="walletAddressId">The wallet address the transactions are for</param>
        /// <param name="transactionType">The type of transaction</param>
        /// <param name="page">The page</param>
        /// <param name="sortOrder">The sort options</param>
        /// <returns>Transactions paged</returns>
        PagedResults<Transaction> GetTransactionsPaged(int userId, string? search, int? walletAddressId, TransactionType? transactionType, Page page, SortOrder sortOrder);

        /// <summary>
        /// Create a new transaction
        /// </summary>
        /// <param name="transactionType">The type of transaction to create</param>
        /// <param name="cryptoCurrencyId">The currency of the transaction</param>
        /// <param name="userId">The user the transaction is for</param>
        /// <param name="hash">The hash of the transaction</param>
        /// <param name="amount">The amount processed</param>
        /// <returns>The new transaction</returns>
        Task<Transaction> CreateTransactionAsync(TransactionType transactionType, int? instructionId, int cryptoCurrencyId, int walletAddressId, int? systemWalletAddressId, int? whitelistAddressId, string? fromAddress, 
            string? toAddress, int userId, string hash, decimal amount, bool reviewed);

        /// <summary>
        /// Get the aggregate balance of transactions
        /// </summary>
        /// <param name="transactionType">The transaction type to filter by</param>
        /// <param name="userId">The user the transactions are for</param>
        /// <param name="walletAddressId">The wallet the transactions are for (optional)</param>
        /// <returns>A balance (aggregate of transactions)</returns>
        StakingBalance AggregateTransactionValue(TransactionType transactionType, int userId, int? walletAddressId);

        /// <summary>
        /// Confirm a transaction and set its processing state
        /// </summary>
        /// <param name="transactionId">The transaction to update</param>
        /// <param name="transactionState">The new transaction state</param>
        /// <returns>The confirmed transaction</returns>
        Task<Transaction> ConfirmTransactionAsync(int transactionId, TransactionState transactionState);

        /// <summary>
        /// Get a transaction by its hash
        /// </summary>
        /// <param name="hash">The transaction hash/id</param>
        /// <returns>A matching internal transaction</returns>
        Transaction? GetTransactionByHash(string hash);

        /// <summary>
        /// Get the transactional balance of an address
        /// </summary>
        /// <param name="walletAddressId">The wallet address to get the transactional balance of</param>
        /// <returns>The aggregate confirmed transactional balance</returns>
        WalletAddressBalance GetBalance(int walletAddressId);

        /// <summary>
        /// Get a transaction by id
        /// </summary>
        /// <param name="transactionId">The transaction to get</param>
        /// <param name="state">The state</param>
        /// <returns>A transaction if exists</returns>
        Transaction? GetTrasaction(int transactionId, ActiveState state);


        /// <summary>
        /// Gets the properties to sort by
        /// </summary>
        /// <returns>All properties to sort by</returns>
        List<(string PropertyName, Order Order)> GetSortProperties();
    }
}
