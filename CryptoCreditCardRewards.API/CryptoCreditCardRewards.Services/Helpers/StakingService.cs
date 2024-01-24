using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Helpers.Interfaces;
using CryptoCreditCardRewards.Services.Http.Interfaces;

namespace CryptoCreditCardRewards.Services.Helpers
{
    public class StakingService : IStakingService
    {
        private readonly IConversionProviderFactory _conversionProviderFactory;
        private readonly ITransactionService _transactionService;

        public StakingService(IConversionProviderFactory conversionProviderFactory, ITransactionService transactionService)
        {
            _conversionProviderFactory = conversionProviderFactory;
            _transactionService = transactionService;
        }

        /// <summary>
        /// Get a users total amount staked in a currency
        /// </summary>
        /// <param name="fromDate">The period staked over</param>
        /// <param name="toDate">The period staked to</param>
        /// <param name="userId">The user to aggregate query is for</param>
        /// <param name="toSymbol">The symbol to aggregate value to</param>
        /// <returns>The aggregate staked value (across wallets) outputted in a specified currency</returns>
        public async Task<decimal> GetStakingAggregateInCurrencyAsync(DateTime fromDate, DateTime toDate, int userId, string toSymbol = "HKD")
        {
            var aggregateStakeValuesInRespectiveCurrencies = await _transactionService.GetUserAggregateStakingTransactionValueAsync(fromDate, toDate, userId);

            // Turn them all to HKD + aggregate
            var conversionService = _conversionProviderFactory.GetConversionService(ConversionServiceType.CryptoCompare);
            var aggregateStakeAmount = 0m;
            foreach (var aggregateStakeValue in aggregateStakeValuesInRespectiveCurrencies)
            {
                var convertedValue = await conversionService.ConvertAsync(aggregateStakeValue.Amount, aggregateStakeValue.Currency, toSymbol);
                aggregateStakeAmount += (convertedValue?.Value ?? 0);
            }

            return aggregateStakeAmount;
        }
    }
}
