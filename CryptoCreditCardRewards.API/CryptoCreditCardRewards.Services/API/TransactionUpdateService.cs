using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.Services.API
{
    public class TransactionUpdateService : ITransactionUpdateService
    {
        private readonly ITransactionService _transactionService;

        public TransactionUpdateService(ITransactionService transactionService)
        {
            _transactionService = transactionService;
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
            // Get the transaction
            var transaction = _transactionService.GetTrasaction(transactionId, ActiveState.Active);
            if (transaction == null)
                throw new NotFoundException(FailedReason.TrasactionDoesntExist, Property.Id);

            return await _transactionService.ReviewTransactionAsync(transactionId, failed, failedReason);
        }
    }
}
