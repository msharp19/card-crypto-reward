using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface ICryptoCurrencyUpdateService
    {
        /// <summary>
        /// Update a crypto currencies active state
        /// </summary>
        /// <param name="cryptoCurrencyId">The id of the crypto currency to update</param>
        /// <returns>The updated crypto currency</returns>
        Task<CryptoCurrency> DeactivateCryptoCurrencyAsync(int cryptoCurrencyId);

        /// <summary>
        /// Update a crypto currencies active state
        /// </summary>
        /// <param name="cryptoCurrencyId">The id of the crypto currency to update</param>
        /// <returns>The updated crypto currency</returns>
        Task<CryptoCurrency> ReactivateCryptoCurrencyAsync(int cryptoCurrencyId);
    }
}
