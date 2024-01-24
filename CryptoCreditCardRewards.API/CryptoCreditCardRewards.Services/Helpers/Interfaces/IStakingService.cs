using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Services.Helpers.Interfaces
{
    public interface IStakingService
    {
        /// <summary>
        /// Get a users total amount staked in a currency
        /// </summary>
        /// <param name="fromDate">The period staked over</param>
        /// <param name="toDate">The period staked to</param>
        /// <param name="userId">The user to aggregate query is for</param>
        /// <param name="toSymbol">The symbol to aggregate value to</param>
        /// <returns>The aggregate staked value (across wallets) outputted in a specified currency</returns>
        Task<decimal> GetStakingAggregateInCurrencyAsync(DateTime fromDate, DateTime toDate, int userId, string toSymbol = "HKD");
    }
}
