using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Services.Entity.Interfaces
{
    public interface IWalletAddressService
    {
        /// <summary>
        /// Get a crypto currency wallet address for a user
        /// </summary>
        /// <param name="userId">The user to get the wallet address for</param>
        /// <param name="cryptoCurrencyId">The crypto currency to get the wallet address for</param>
        /// <param name="state">The state of the wallet address</param>
        /// <returns>A wallet address for currency/user</returns>
        WalletAddress? GetUsersCurrencyWalletAddress(int userId, int cryptoCurrencyId, ActiveState state = ActiveState.Active);

        /// <summary>
        /// Create a wallet address for a user/currency
        /// </summary>
        /// <param name="userId">The user to create wallet address for</param>
        /// <param name="cryptoCurrencyId">The currency to create the wallet address for</param>
        /// <returns>A new wallet address</returns>
        Task<WalletAddress> CreateWalletAddressAsync(int userId, int cryptoCurrencyId);

        /// <summary>
        /// Checks if a user has an address setup for a crypto currency
        /// </summary>
        /// <param name="cryptoCurrencyId">The crypto currency to check for</param>
        /// <param name="userId">The user to check for</param>
        /// <returns>True if a user has an address for the provided crypto currency</returns>
        bool DoesUserOwnCryptoAddress(int cryptoCurrencyId, int userId);

        /// <summary>
        /// Get a wallet address by its id
        /// </summary>
        /// <param name="walletAddressId">The wallet address to get</param>
        /// <returns>A wallet address</returns>
        WalletAddress? GetWalletAddressById(int walletAddressId);

        /// <summary>
        /// Gets the properties to sort by
        /// </summary>
        /// <returns>All properties to sort by</returns>
        List<(string PropertyName, Order Order)> GetSortProperties();

        /// <summary>
        /// Get wallet addresses paged
        /// </summary>
        /// <param name="search">A search term (address/currency etc.)</param>
        /// <param name="userId">The user the wallets are for</param>
        /// <param name="cryptoCurrencyId">The crypto currency the wallets are for</param>
        /// <param name="page">The page to return</param>
        /// <param name="sortOrder">The order returned in</param>
        /// <returns>Paged wallet addresses</returns>
        PagedResults<WalletAddress> GetWalletAddressesPaged(string? search, int? userId, int? cryptoCurrencyId, ActiveState? state, Page page, SortOrder sortOrder);

        /// <summary>
        /// Get wallet addresses for a user/crypto currencies
        /// </summary>
        /// <param name="userId">The user to get wallet addresses for</param>
        /// <param name="cryptoCurrencyIds">The crypto currencies to get wallet addresses for</param>
        /// <returns>Wallet addresses</returns>
        List<WalletAddress> GetWalletAddresses(int userId, List<int> cryptoCurrencyIds);

        /// <summary>
        /// Get a wallet address by its id
        /// </summary>
        /// <param name="walletAddressId">The wallet address to get</param>
        /// <returns>A wallet address</returns>
        WalletAddress? GetWalletAddress(int walletAddressId, ActiveState state = ActiveState.Active);
    }
}
