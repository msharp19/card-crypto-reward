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
    public class SystemWalletAddressCreateService : ISystemWalletAddressCreateService
    {
        private readonly ISystemWalletAddressService _systemWalletAddressService;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;

        public SystemWalletAddressCreateService(ISystemWalletAddressService systemWalletAddressService, ICryptoCurrencyService cryptoCurrencyService)
        {
            _systemWalletAddressService = systemWalletAddressService;
            _cryptoCurrencyService = cryptoCurrencyService;
        }

        /// <summary>
        /// Create a system wallet address for a currency
        /// </summary>
        /// <param name="cryptoCurrencyId">The currency to create the wallet address for</param>
        /// <param name="addressType">The address type</param>
        /// <returns>A new wallet address</returns>
        public async Task<SystemWalletAddress> CreateSystemWalletAddressAsync(int cryptoCurrencyId, AddressType addressType)
        {
            // Get crypto
            var crypto = _cryptoCurrencyService.GetCryptoCurrency(cryptoCurrencyId);
            if (crypto == null)
                throw new NotFoundException(FailedReason.CryptoCurrencyDoesntExist, Property.CryptoCurrencyId);

            // Try to add new wallet address
            return await _systemWalletAddressService.CreateSystemWalletAddressAsync(cryptoCurrencyId, addressType);
        }
    }
}
