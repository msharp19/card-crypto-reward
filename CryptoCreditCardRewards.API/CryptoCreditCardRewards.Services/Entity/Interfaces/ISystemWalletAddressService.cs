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
    public interface ISystemWalletAddressService
    {
        /// <summary>
        /// Get a system crypto currency wallet address
        /// </summary>
        /// <param name="cryptoCurrencyId">The crypto currency to get the wallet address for</param>
        /// <param name="state">The state of the wallet address</param>
        /// <returns>A wallet address for currency/user</returns>
        SystemWalletAddress? GetSystemWalletAddress( int cryptoCurrencyId, AddressType addressType, ActiveState state = ActiveState.Active);

        /// <summary>
        /// Create a system wallet address for a currency
        /// </summary>
        /// <param name="cryptoCurrencyId">The currency to create the wallet address for</param>
        /// <param name="addressType">The address type</param>
        /// <returns>A new wallet address</returns>
        Task<SystemWalletAddress> CreateSystemWalletAddressAsync(int cryptoCurrencyId, AddressType addressType);

        /// <summary>
        /// Get a system wallet address by its id
        /// </summary>
        /// <param name="walletAddressId">The wallet address to get</param>
        /// <returns>A wallet address</returns>
        SystemWalletAddress? GetSystemWalletAddressById(int walletAddressId, ActiveState state = ActiveState.Active);

        /// <summary>
        /// Gets the properties to sort by
        /// </summary>
        /// <returns>All properties to sort by</returns>
        List<(string PropertyName, Order Order)> GetSortProperties();

        /// <summary>
        /// Get system wallet addresses paged
        /// </summary>
        /// <param name="search">A search term (address/currency etc.)</param>
        /// <param name="cryptoCurrencyId">The crypto currency the wallets are for</param>
        /// <param name="page">The page to return</param>
        /// <param name="sortOrder">The order returned in</param>
        /// <returns>Paged system wallet addresses</returns>
        PagedResults<SystemWalletAddress> GetSystemWalletAddressesPaged(string? search, int? cryptoCurrencyId, ActiveState? state, Page page, SortOrder sortOrder);
        
        /// <summary>
        /// Get system wallet addresses for address type/currencies
        /// </summary>
        /// <param name="cryptoCurrencyIds">The currencies to get system wallets for</param>
        /// <param name="addressType">The address type</param>
        /// <param name="state">If the wallet addresses are active or not</param>
        /// <returns>System wallet addresses</returns>
        List<SystemWalletAddress> GetSystemWalletAddresses(List<int> cryptoCurrencyIds, AddressType addressType, ActiveState state);
    }
}
