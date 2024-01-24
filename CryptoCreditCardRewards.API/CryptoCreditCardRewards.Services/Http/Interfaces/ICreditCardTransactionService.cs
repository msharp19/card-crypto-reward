using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Dtos.CreditCardTransactions;

namespace CryptoCreditCardRewards.Services.Http.Interfaces
{
    public interface ICreditCardTransactionService
    {
        /// <summary>
        /// Get the aggregate monthly spend for an account
        /// </summary>
        /// <param name="fromDate">The date the value derived is from</param>
        /// <param name="toDate">The date the value derived is to</param>
        /// <param name="accountNumber">The account to get the monthly spend for</param>
        /// <returns>The monthly spend for an account</returns>
        Task<AggregateTransactionValueDto> GetAggregateTransactionValueAsync(DateTime fromDate, DateTime toDate, string accountNumber, string currency = "HKD");
    }
}
