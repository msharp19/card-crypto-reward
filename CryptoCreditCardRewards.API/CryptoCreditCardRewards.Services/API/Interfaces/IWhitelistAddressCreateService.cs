using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface IWhitelistAddressCreateService
    {
        /// <summary>
        /// Add an address to whitelist
        /// </summary>
        /// <param name="userId">The user the address is for</param>
        /// <param name="cryptoCurrencyId">The crypto currency the address is for</param>
        /// <param name="address">The address</param>
        /// <returns>A new whitelist address</returns>
        Task<WhitelistAddress> CreateWhitelistAddressAsync(int userId, int cryptoCurrencyId, string address);
    }
}
