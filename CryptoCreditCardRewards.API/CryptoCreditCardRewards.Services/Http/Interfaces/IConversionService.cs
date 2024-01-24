using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Misc;

namespace CryptoCreditCardRewards.Services.Http.Interfaces
{
    public interface IConversionService
    {
        /// <summary>
        /// Convert a currency from->to
        /// </summary>
        /// <param name="amount">The amount to convert</param>
        /// <param name="fromSymbol">The symbol from</param>
        /// <param name="toSymbol">The symbol to</param>
        /// <returns>The converted currency</returns>
        Task<CurrencyConversion> ConvertAsync(decimal amount, string fromSymbol, string toSymbol);

        /// <summary>
        /// Checks if symbol is supported
        /// </summary>
        /// <param name="symbol">The symbol to check is supported</param>
        /// <returns>True if symbol is supported for implementation</returns>
        Task<bool> IsSupportedAsync(string? symbol);
    }
}
