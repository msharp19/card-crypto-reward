using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface IWalletCreateService
    {
        /// <summary>
        /// Add a wallet address for a user account
        /// </summary>
        /// <param name="userId">The user to add wallet address for</param>
        /// <param name="cryptoCurrencyId">The crypto currency to add to wallet</param>
        /// <returns>The new wallet address created</returns>
        Task<WalletAddress> AddWalletAddressToUserAsync(int userId, int cryptoCurrencyId);
    }
}
