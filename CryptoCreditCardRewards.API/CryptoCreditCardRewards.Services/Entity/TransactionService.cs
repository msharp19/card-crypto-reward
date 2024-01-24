using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Contexts;
using CryptoCreditCardRewards.Models.Dtos.CreditCardTransactions;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Models.Misc;
using CryptoCreditCardRewards.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoCreditCardRewards.Services.Entity
{
    public class TransactionService : ITransactionService
    {
        private readonly CryptoCreditCardRewardsDbContext _context;

        public TransactionService(CryptoCreditCardRewardsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the aggregate transaction value
        /// </summary>
        /// <param name="fromDate">The date the aggregate value is from</param>
        /// <param name="toDate">The date the aggregate value is to</param>
        /// <param name="userId">The user the aggregate transaction value is for</param>
        /// <returns>An aggregate transaction value over a period for a user</returns>
        public async Task<List<AggregateTransactionValueDto>> GetUserAggregateStakingTransactionValueAsync(DateTime fromDate, DateTime toDate, int userId)
        {
            // Get the total being staked
            var aggregates = _context.Transactions.Include(x => x.CryptoCurrency)
                .Where(x => x.Active)
                .Where(x => x.Type == TransactionType.Staking)
                .Where(x => x.UserId == userId)
                // Only include -ve transactions or +ve transactions before the fromDate (this ensures that only staking values that have been around for 1m can be used) 
                //.Where(x => (x.Amount < 0) || (x.Amount > 0 && x.CreatedDate < fromDate))
                .GroupBy(x => x.CryptoCurrency)
                .Select(x => new AggregateTransactionValueDto()
                {
                    Amount = x.Sum(y => y.Amount),
                    Currency = x.Key.Symbol.ToUpper().Trim(),
                    CryptoCurrencyId = x.Key.Id
                })
                .ToList();

            return aggregates;
        }

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
        public PagedResults<Transaction> GetTransactionsPaged(int userId, string? search, int? walletAddressId, TransactionType? transactionType, Page page, SortOrder sortOrder)
        {
            // Get transactions query
            var transactions = GetTransactionsQuery(userId, search, walletAddressId, transactionType);

            // Sort 
            transactions = OrderTransactions(transactions, sortOrder);

            // Paginate
            var results = transactions.Skip((int)(page.PageIndex * page.PerPage))
                .Take((int)page.PerPage)
                .ToList();

            // Get total
            var total = GetTransactionsQuery(userId, search, walletAddressId, transactionType).Select(x => x.Id).Count();

            // Map and return
            return new PagedResults<Transaction>()
            {
                Items = results,
                Page = page,
                SortOrder = sortOrder,
                TotalCount = total
            };
        }

        /// <summary>
        /// Get a list of unconfirmed transactions
        /// </summary>
        /// <returns>Unconfirmed transaction</returns>
        public List<Transaction> GetUnconfirmedTransactions()
        {
            return _context.Transactions
                .Where(x => x.ConfirmedDate == null)
                .ToList();
        }

        /// <summary>
        /// Review a transaction
        /// </summary>
        /// <param name="transactionId">The transaction</param>
        /// <param name="failed">If the review failed</param>
        /// <param name="failedReason">The reason for failure (if any)</param>
        /// <returns>The reviewed transaction</returns>
        public async Task<Transaction> ReviewTransactionAsync(int transactionId, bool failed, string? failedReason)
        {
            // Get the transaction to update
            var transaction = _context.Transactions.FirstOrDefault(x => x.Id == transactionId);

            transaction.Review(failedReason, failed);

            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        /// <summary>
        /// Create a new transaction
        /// </summary>
        /// <param name="transactionType">The type of transaction to create</param>
        /// <param name="cryptoCurrencyId">The currency of the transaction</param>
        /// <param name="userId">The user the transaction is for</param>
        /// <param name="hash">The hash of the transaction</param>
        /// <param name="amount">The amount processed</param>
        /// <returns>The new transaction</returns>
        public async Task<Transaction> CreateTransactionAsync(TransactionType transactionType, int? instructionId, int cryptoCurrencyId, int walletAddressId, int? systemWalletAddressId, int? whitelistAddressId,
            string? fromAddress, string? toAddress, int userId, string hash, decimal amount, bool reviewed)
        {
            // Build
            var transaction = new Transaction(true, transactionType, instructionId, cryptoCurrencyId, walletAddressId, systemWalletAddressId, whitelistAddressId, fromAddress, toAddress, userId, hash, amount);

            // If reviewed on creation, nothing to add for notes
            if (reviewed)
            {
                transaction.Review(null, false);
            }

            // Create & save
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Return
            return transaction;
        }

        /// <summary>
        /// Get the aggregate balance of transactions
        /// </summary>
        /// <param name="transactionType">The transaction type to filter by</param>
        /// <param name="userId">The user the transactions are for</param>
        /// <param name="walletAddressId">The wallet the transactions are for (optional)</param>
        /// <returns>A balance (aggregate of transactions)</returns>
        public StakingBalance AggregateTransactionValue(TransactionType transactionType, int userId, int? walletAddressId)
        {
            var transactions = _context.Transactions.Where(x => x.Active)
                .Where(x => x.Type == transactionType)
                .Where(x => x.UserId == userId);

            // Filter by wallet id
            if (walletAddressId.HasValue)
            {
                // Get all users wallets
                var allUsersWalletCryptoIds = _context.WalletAddresses
                    .Where(x => x.Active)
                    .Where(x => x.UserId == userId)
                    .Select(x => x.CryptoCurrencyId)
                    .ToList();

                // Filter
                transactions = transactions.Where(x => allUsersWalletCryptoIds.Contains(x.CryptoCurrencyId));
            }

            return new StakingBalance()
            {
                ConfirmedBalance = transactions.Where(x => x.ConfirmedDate != null).Sum(x => x.Amount),
                UnconfirmedBalance = transactions.Where(x => x.ConfirmedDate == null).Sum(x => x.Amount),
            };
        }

        /// <summary>
        /// Confirm a transaction and set its processing state
        /// </summary>
        /// <param name="transactionId">The transaction to update</param>
        /// <param name="transactionState">The new transaction state</param>
        /// <returns>The confirmed transaction</returns>
        public async Task<Transaction> ConfirmTransactionAsync(int transactionId, TransactionState transactionState)
        {
            // Get the transaction to update
            var transaction = _context.Transactions.FirstOrDefault(x => x.Id == transactionId);

            transaction.Confirm(DateTime.UtcNow, transactionState);

            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        /// <summary>
        /// Get a transaction by its hash
        /// </summary>
        /// <param name="hash">The transaction hash/id</param>
        /// <returns>A matching internal transaction</returns>
        public Transaction? GetTransactionByHash(string hash)
        {
            return _context.Transactions.FirstOrDefault(x => x.Hash == hash);
        }

        /// <summary>
        /// Get the transactional balance of an address
        /// </summary>
        /// <param name="walletAddressId">The wallet address to get the transactional balance of</param>
        /// <returns>The aggregate confirmed transactional balance</returns>
        public WalletAddressBalance GetBalance(int walletAddressId)
        {
            var wallet = _context.WalletAddresses.FirstOrDefault(x => x.Id == walletAddressId);

            // Enumerate so we dont keep making queries
            var transactions = _context.Transactions.Where(x => x.WalletAddressId == walletAddressId)
                .ToList();

            var spendableBalance = transactions.Where(x => x.ConfirmedDate != null).Where(x => x.ReviewedDate != null).Where(x => x.FailedReview == false).Sum(x => (x.Type == TransactionType.Staking) ? (-1 * x.Amount) : x.Amount);
            var confirmedBalance = transactions.Where(x => x.ConfirmedDate != null).Sum(x => (x.Type == TransactionType.Staking) ? (-1 * x.Amount) : x.Amount);
            var unconfirmedBalance = transactions.Where(x => x.ConfirmedDate == null).Sum(x => (x.Type == TransactionType.Staking) ? (-1 * x.Amount) : x.Amount);
            var successfullyReviewdBalance = transactions.Where(x => x.ReviewedDate != null).Where(x => x.FailedReview == false).Sum(x => (x.Type == TransactionType.Staking) ? (-1 * x.Amount) : x.Amount);
            var unsuccessfullyReviewedBalance = transactions.Where(x => x.ReviewedDate != null).Where(x => x.FailedReview == true).Sum(x => (x.Type == TransactionType.Staking) ? (-1 * x.Amount) : x.Amount);
            var unreviewedBalance = transactions.Where(x => x.ReviewedDate == null).Sum(x => (x.Type == TransactionType.Staking) ? (-1 * x.Amount) : x.Amount);

            var stakedBalance = transactions.Where(x => x.ConfirmedDate != null).Where(x => x.Type == TransactionType.Staking).Sum(x => x.Amount);
            var unconfirmedStakedBalance = transactions.Where(x => x.ConfirmedDate == null).Where(x => x.Type == TransactionType.Staking).Sum(x => x.Amount);

            var instructions = _context.Instructions.Where(x => x.WalletAddressId == walletAddressId || x.UserId == wallet.UserId).Where(x => x.Active).Where(x => x.CompletedDate == null).ToList();
            var outstandingInstructionBalance = instructions.Sum(x => x.Type == InstructionType.StakingDeposit || x.Type == InstructionType.StakingWithdrawal ? (-1 * x.Amount) : x.Amount);
            var outstandingInstructionStakedBalance = instructions.Where(x => x.Type == InstructionType.StakingWithdrawal || x.Type == InstructionType.StakingDeposit).Sum(x => x.Amount);

            return new WalletAddressBalance()
            {
                SpendableBalance = spendableBalance,
                ConfirmedBalance = confirmedBalance,
                UnconfirmedBalance = unconfirmedBalance,
                SuccessfullyReviewedBalance = successfullyReviewdBalance,
                UnsuccessfullyReviewedBalance = unsuccessfullyReviewedBalance,
                UnReviewedBalance = unreviewedBalance,
                SpendableStakedBalance = stakedBalance,
                UnconfirmedStakedBalance = unconfirmedStakedBalance,
                OutstandingInstructionBalance = outstandingInstructionBalance,
                OutstandingInstructionStakedBalance = outstandingInstructionStakedBalance,
            };
        }

        /// <summary>
        /// Get a transaction by id
        /// </summary>
        /// <param name="transactionId">The transaction to get</param>
        /// <param name="state">The state</param>
        /// <returns>A transaction if exists</returns>
        public Transaction? GetTrasaction(int transactionId, ActiveState state)
        {
            var transactions = _context.Transactions.Include(x => x.CryptoCurrency)
                .Include(x => x.SystemWalletAddress)
                .Include(x => x.WhitelistAddress)
                .Include(x => x.User)
                .Include(x => x.WalletAddress)
                .Where(x => x.Id == transactionId);

            if (state == ActiveState.Active)
                transactions = transactions.Where(x => x.Active);
            else if (state == ActiveState.InActive)
                transactions = transactions.Where(x => !x.Active);

            return transactions.FirstOrDefault();
        }

        /// <summary>
        /// Gets the properties to sort by
        /// </summary>
        /// <returns>All properties to sort by</returns>
        public List<(string PropertyName, Order Order)> GetSortProperties()
        {
            return new List<(string PropertyName, Order Order)>()
            {
                new ("createdDate", Order.Ascending),
                new ("createdDate", Order.Descending),
            };
        }

        #region Helpers

        public IQueryable<Transaction> GetTransactionsQuery(int? userId, string? search, int? walletAddressId, TransactionType? transactionType)
        {
            // Get the transactions
            var transactions = _context.Transactions.Include(x => x.CryptoCurrency)
                .Include(x => x.SystemWalletAddress)
                .Include(x => x.WhitelistAddress)
                .Include(x => x.User)
                .Include(x => x.WalletAddress)
                .AsQueryable();

            // Filter by user
            if (userId.HasValue)
            {
                transactions = transactions.Where(x => x.UserId == userId);
            }

            // Filter by wallet
            if (walletAddressId.HasValue)
            {
                transactions = transactions.Where(x => x.WalletAddressId == walletAddressId);
            }

            // Filter by user
            if (transactionType.HasValue)
            {
                transactions = transactions.Where(x => x.Type == transactionType);
            }

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();

                transactions = transactions.Where(x => x.CryptoCurrency != null)
                    .Where(x => x.CryptoCurrency.Name.ToLower().Contains(search) ||
                             x.CryptoCurrency.Symbol.ToLower().Contains(search));
            }

            return transactions;
        }

        /// <summary>
        /// Orders transactions in a queryable list
        /// </summary>
        /// <param name="transactions">The list to order</param>
        /// <param name="sortOrder">The sort order details</param>
        /// <returns>Sorted transaction list</returns>
        private IQueryable<Transaction> OrderTransactions(IQueryable<Transaction> transactions, SortOrder sortOrder)
        {
            // Sort users where supported property exists - default name
            return (sortOrder.OrderProperty.Trim(), sortOrder.Order) switch
            {
                ("createdDate", Order.Ascending) => transactions.OrderBy(x => x.CreatedDate),
                ("createdDate", Order.Descending) => transactions.OrderByDescending(x => x.CreatedDate),
                _ => sortOrder.Order == Order.Ascending ? transactions.OrderBy(x => x.CreatedDate) : transactions.OrderByDescending(x => x.CreatedDate)
            };
        }

        #endregion
    }
}
