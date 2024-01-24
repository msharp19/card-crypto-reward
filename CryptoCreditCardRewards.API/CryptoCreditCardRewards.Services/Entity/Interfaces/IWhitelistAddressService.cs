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
    public interface IWhitelistAddressService
    {
        /// <summary>
        /// Get a whitelist address
        /// </summary>
        /// <param name="id">The id of the whitelist address</param>
        /// <returns>The whitelisted address</returns>
        WhitelistAddress? GetWhitelistAddress(int id, WhitelistAddressState state);

        /// <summary>
        /// Get a whitelist address by address
        /// </summary>
        /// <param name="id">The address</param>
        /// <returns>The whitelisted address</returns>
        WhitelistAddress? GetWhitelistAddress(int userId, string address, WhitelistAddressState state);

        /// <summary>
        /// Gets the properties to sort by
        /// </summary>
        /// <returns>All properties to sort by</returns>
        List<(string PropertyName, Order Order)> GetSortProperties();

        /// <summary>
        /// Get whitelist addresses paged
        /// </summary>
        /// <param name="search">A search term</param>
        /// <param name="vaildated"></param>
        /// <param name="processedDateFrom">From date the address has been processed (validated) at</param>
        /// <param name="processedDateTo">To date the address has been processed (validated) at</param>
        /// <param name="cryptoCurrencyId">The crypto currency</param>
        /// <param name="userId">The user the address is whitelisted for</param>
        /// <param name="sortOrder">The order to order results by</param>
        /// <param name="address">The whitelist address</param>
        /// <param name="page">The page to return</param>
        /// <returns>Page of whitelisted addresses</returns>
        PagedResults<WhitelistAddress> GetWhitelistAddressesPaged(string? search, int? userId, int? cryptoCurrencyId, bool? vaildated, DateTime? processedDateFrom, DateTime? processedDateTo, string? address,
            Page page, SortOrder sortOrder);

        /// <summary>
        /// Validate a whitelist address (mark as valid/invalid)
        /// </summary>
        /// <param name="id">The whitelist address to update</param>
        /// <param name="isValid">If the address is valid</param>
        /// <param name="failedReason">A reason the validation failed (if any)</param>
        /// <returns>The updated whitelist address</returns>
        Task<WhitelistAddress> ValidateWhitelistAddressAsync(int id, bool isValid, string? failedReason);

        /// <summary>
        /// Checks if an address has already been whitelisted for a user
        /// </summary>
        /// <param name="userId">The user top check for</param>
        /// <param name="address">The address to check</param>
        /// <returns>True if already whitelisted for the user</returns>
        bool IsAddressAlreadyWhitelisted(int userId, string address);

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
