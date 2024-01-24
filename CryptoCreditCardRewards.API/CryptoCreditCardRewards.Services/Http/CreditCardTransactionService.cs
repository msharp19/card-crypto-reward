using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Dtos.CreditCardTransactions;
using CryptoCreditCardRewards.Services.Http.Interfaces;

namespace CryptoCreditCardRewards.Services.Http
{
    public class CreditCardTransactionService : BaseHttpService, ICreditCardTransactionService
    {
        public CreditCardTransactionService(HttpClient client) : base(client)
        {
        }

        /// <summary>
        /// Get the aggregate monthly spend for an account
        /// </summary>
        /// <param name="fromDate">The date the value derived is from</param>
        /// <param name="toDate">The date the value derived is to</param>
        /// <param name="accountNumber">The account to get the monthly spend for</param>
        /// <returns>The monthly spend for an account</returns>
        public async Task<AggregateTransactionValueDto> GetAggregateTransactionValueAsync(DateTime fromDate, DateTime toDate,
            string accountNumber, string currency = "HKD")
        {
            // Get authentication
            var headers = await GetAuthenticatedHeadersAsync();

            // Make request
            //return await GetAsync<MonthlyTransactionValueDto>($"user/{accountId}/currency/{currency}/total?dateFrom={dateFrom}&dateTo={dateTo}", headers);

            // TODO: This is stubbed until we have access to an API (when we know what API method to implement).
            // This generates a random value for the response in USD
            return new AggregateTransactionValueDto() { Amount = new Random().Next(5000), Currency = "HKD" };
        }

        #region Helpers

        /// <summary>
        /// Get an access token to query the Zoom API with
        /// </summary>
        /// <returns>Access token</returns>
        public override async Task<string> GetAccessTokenAsync()
        {
            // TODO: This is stubbed until we have access to an API (when we know what authentication method to implement)
            return "token";
        }

        #endregion
    }
}
