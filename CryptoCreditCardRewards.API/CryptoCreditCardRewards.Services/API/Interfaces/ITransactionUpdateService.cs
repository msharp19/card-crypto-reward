using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface ITransactionUpdateService
    {
        /// <summary>
        /// Review a transaction
        /// </summary>
        /// <param name="transactionId">The transaction</param>
        /// <param name="failed">If the review failed</param>
        /// <param name="failedReason">The reason for failure (if any)</param>
        /// <returns>The reviewed transaction</returns>
        Task<Transaction> ReviewTransactionAsync(int transactionId, bool failed, string? failedReason);
    }
}
