using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.Services.API
{
    public class CryptoCurrencyUpdateService : ICryptoCurrencyUpdateService
    {
        private readonly ICryptoCurrencyService _cryptoCurrencyService;

        public CryptoCurrencyUpdateService(ICryptoCurrencyService cryptoCurrencyService)
        {
            _cryptoCurrencyService = cryptoCurrencyService;
        }

        /// <summary>
        /// Update a crypto currencies active state
        /// </summary>
        /// <param name="cryptoCurrencyId">The id of the crypto currency to update</param>
        /// <returns>The updated crypto currency</returns>
        public async Task<CryptoCurrency> DeactivateCryptoCurrencyAsync(int cryptoCurrencyId)
        {
            // Get crypto
            var crypto = _cryptoCurrencyService.GetCryptoCurrency(cryptoCurrencyId);
            if (crypto == null)
                throw new NotFoundException(FailedReason.CryptoCurrencyDoesntExist, Property.CryptoCurrencyId);

            return await _cryptoCurrencyService.UpdateCryptoCurrencyStateAsync(cryptoCurrencyId, false);
        }

        /// <summary>
        /// Update a crypto currencies active state
        /// </summary>
        /// <param name="cryptoCurrencyId">The id of the crypto currency to update</param>
        /// <returns>The updated crypto currency</returns>
        public async Task<CryptoCurrency> ReactivateCryptoCurrencyAsync(int cryptoCurrencyId)
        {
            // Get crypto
            var crypto = _cryptoCurrencyService.GetCryptoCurrency(cryptoCurrencyId, ActiveState.Both);
            if (crypto == null)
                throw new NotFoundException(FailedReason.CryptoCurrencyDoesntExist, Property.CryptoCurrencyId);

            return await _cryptoCurrencyService.UpdateCryptoCurrencyStateAsync(cryptoCurrencyId, true);
        }
    }
}
