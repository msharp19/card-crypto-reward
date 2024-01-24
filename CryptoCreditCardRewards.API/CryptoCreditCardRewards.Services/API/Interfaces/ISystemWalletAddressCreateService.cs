using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface ISystemWalletAddressCreateService
    {
        /// <summary>
        /// Create a system wallet address for a currency
        /// </summary>
        /// <param name="cryptoCurrencyId">The currency to create the wallet address for</param>
        /// <param name="addressType">The address type</param>
        /// <returns>A new wallet address</returns>
        Task<SystemWalletAddress> CreateSystemWalletAddressAsync(int cryptoCurrencyId, AddressType addressType);
    }
}
