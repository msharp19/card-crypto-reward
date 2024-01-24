using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Contexts;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.Services.Entity
{
    public class WhitelistAddressService : IWhitelistAddressService
    {
        private readonly CryptoCreditCardRewardsDbContext _context;

        public WhitelistAddressService(CryptoCreditCardRewardsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a whitelist address
        /// </summary>
        /// <param name="id">The id of the whitelist address</param>
        /// <returns>The whitelisted address</returns>
        public WhitelistAddress? GetWhitelistAddress(int id, WhitelistAddressState state)
        {
            var whitelistAddresses = _context.WhitelistAddresses.Where(x => x.Id == id);

            if (state == WhitelistAddressState.UnProcessed)
                whitelistAddresses = whitelistAddresses.Where(x => x.ProcessedDate == null);
            if (state == WhitelistAddressState.Vaild)
                whitelistAddresses = whitelistAddresses.Where(x => x.Valid && x.ProcessedDate != null);
            if (state == WhitelistAddressState.Invalid)
                whitelistAddresses = whitelistAddresses.Where(x => !x.Valid && x.ProcessedDate != null);

            return whitelistAddresses.FirstOrDefault();
        }

        /// <summary>
        /// Get a whitelist address by address
        /// </summary>
        /// <param name="id">The address</param>
        /// <returns>The whitelisted address</returns>
        public WhitelistAddress? GetWhitelistAddress(int userId, string address, WhitelistAddressState state)
        {
            var whitelistAddresses = _context.WhitelistAddresses.Where(x => x.UserId == userId)
                .Where(x => x.Address.ToLower().Trim() == address.ToLower().Trim());

            if (state == WhitelistAddressState.UnProcessed)
                whitelistAddresses = whitelistAddresses.Where(x => x.ProcessedDate == null);
            if (state == WhitelistAddressState.Vaild)
                whitelistAddresses = whitelistAddresses.Where(x => x.Valid && x.ProcessedDate != null);
            if (state == WhitelistAddressState.Invalid)
                whitelistAddresses = whitelistAddresses.Where(x => !x.Valid && x.ProcessedDate != null);

            return whitelistAddresses.FirstOrDefault();
        }

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
        public PagedResults<WhitelistAddress> GetWhitelistAddressesPaged(string? search, int? userId, int? cryptoCurrencyId, bool? vaildated, DateTime? processedDateFrom, DateTime? processedDateTo, string? address,
            Page page, SortOrder sortOrder)
        {
            // Get white list address query
            var users = GetWhitelistAddressQuery(search, userId, cryptoCurrencyId, vaildated, processedDateFrom, processedDateTo, address);

            // Sort 
            users = OrderWhitelistAddresses(users, sortOrder);

            // Paginate
            var results = users.Skip((int)(page.PageIndex * page.PerPage))
                .Take((int)page.PerPage)
                .ToList();

            // Get total
            var total = GetWhitelistAddressQuery(search, userId, cryptoCurrencyId, vaildated, processedDateFrom, processedDateTo, address).Select(x => x.Id).Count();

            // Map and return
            return new PagedResults<WhitelistAddress>()
            {
                Items = results,
                Page = page,
                SortOrder = sortOrder,
                TotalCount = total
            };
        }

        /// <summary>
        /// Validate a whitelist address (mark as valid/invalid)
        /// </summary>
        /// <param name="id">The whitelist address to update</param>
        /// <param name="isValid">If the address is valid</param>
        /// <param name="failedReason">A reason the validation failed (if any)</param>
        /// <returns>The updated whitelist address</returns>
        public async Task<WhitelistAddress> ValidateWhitelistAddressAsync(int id, bool isValid, string? failedReason)
        {
            var whitelistAddress = _context.WhitelistAddresses.FirstOrDefault(x => x.Id == id);

            whitelistAddress.Process(isValid, failedReason);

            _context.WhitelistAddresses.Update(whitelistAddress);
            await _context.SaveChangesAsync();

            return whitelistAddress;
        }

        /// <summary>
        /// Checks if an address has already been whitelisted for a user
        /// </summary>
        /// <param name="userId">The user top check for</param>
        /// <param name="address">The address to check</param>
        /// <returns>True if already whitelisted for the user</returns>
        public bool IsAddressAlreadyWhitelisted(int userId, string address)
        {
            return _context.WhitelistAddresses.Where(x => x.UserId == userId)
                .Where(x => x.Address.ToLower().Trim() == address.ToLower().Trim())
                .Any();
        }

        /// <summary>
        /// Add an address to whitelist
        /// </summary>
        /// <param name="userId">The user the address is for</param>
        /// <param name="cryptoCurrencyId">The crypto currency the address is for</param>
        /// <param name="address">The address</param>
        /// <returns>A new whitelist address</returns>
        public async Task<WhitelistAddress> CreateWhitelistAddressAsync(int userId, int cryptoCurrencyId, string address)
        {
            var newWhitelistAddress = new WhitelistAddress(userId, cryptoCurrencyId, address);

            _context.WhitelistAddresses.Add(newWhitelistAddress);
            await _context.SaveChangesAsync();

            return newWhitelistAddress;
        }

        /// <summary>
        /// Gets the properties to sort by
        /// </summary>
        /// <returns>All properties to sort by</returns>
        public List<(string PropertyName, Order Order)> GetSortProperties()
        {
            return new List<(string PropertyName, Order Order)>()
            {
                new ("createdDate", Order.Ascending),
                new ("createdDate", Order.Descending),
                new ("address", Order.Ascending),
                new ("address", Order.Descending),
                new ("processedDate", Order.Ascending),
                new ("processedDate", Order.Descending),
            };
        }

        #region Helpers

        private IQueryable<WhitelistAddress> GetWhitelistAddressQuery(string? search, int? userId, int? cryptoCurrencyId, bool? validated, DateTime? processedDateFrom, DateTime? processedDateTo, string? address)
        {
            var whitelistAddresses = _context.WhitelistAddresses.AsQueryable();

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearchTerm = search.ToLower().Trim();
                whitelistAddresses = whitelistAddresses.Where(x => x.Address.ToLower().Contains(lowerSearchTerm) ||
                     x.FailedReason.ToLower().Contains(lowerSearchTerm));
            }

            // Filter by email
            if (!string.IsNullOrEmpty(address))
            {
                whitelistAddresses = whitelistAddresses.Where(x => x.Address.ToLower().Trim() == address.ToLower().Trim());
            }

            // Filter by user
            if (userId.HasValue)
            {
                whitelistAddresses = whitelistAddresses.Where(x => x.UserId == userId);
            }

            // Filter by crypto currency
            if (cryptoCurrencyId.HasValue)
            {
                whitelistAddresses = whitelistAddresses.Where(x => x.CryptoCurrencyId == cryptoCurrencyId);
            }

            // Filter by validated or not
            if (validated.HasValue)
            {
                whitelistAddresses = whitelistAddresses.Where(x => x.Valid == validated);
            }

            // Filter by processed from date
            if (processedDateFrom.HasValue)
            {
                whitelistAddresses = whitelistAddresses.Where(x => x.ProcessedDate != null).Where(x => x.ProcessedDate >= processedDateFrom);
            }

            if (processedDateTo.HasValue)
            {
                whitelistAddresses = whitelistAddresses.Where(x => x.ProcessedDate != null).Where(x => x.ProcessedDate <= processedDateTo);
            }


            return whitelistAddresses;
        }

        /// <summary>
        /// Orders whitelist addresses in a queryable list
        /// </summary>
        /// <param name="whitelistAddresses">The list to order</param>
        /// <param name="sortOrder">The sort order details</param>
        /// <returns>Sorted white list address list</returns>
        private IQueryable<WhitelistAddress> OrderWhitelistAddresses(IQueryable<WhitelistAddress> whitelistAddresses, SortOrder sortOrder)
        {
            // Sort users where supported property exists - default name
            return (sortOrder.OrderProperty.Trim(), sortOrder.Order) switch
            {
                ("createdDate", Order.Ascending) => whitelistAddresses.OrderBy(x => x.CreatedDate),
                ("createdDate", Order.Descending) => whitelistAddresses.OrderByDescending(x => x.CreatedDate),
                ("address", Order.Ascending) => whitelistAddresses.OrderBy(x => x.Address),
                ("address", Order.Descending) => whitelistAddresses.OrderByDescending(x => x.Address),
                ("processedDate", Order.Ascending) => whitelistAddresses.OrderBy(x => x.ProcessedDate),
                ("processedDate", Order.Descending) => whitelistAddresses.OrderByDescending(x => x.ProcessedDate),
                _ => sortOrder.Order == Order.Ascending ? whitelistAddresses.OrderBy(x => x.CreatedDate) : whitelistAddresses.OrderByDescending(x => x.CreatedDate)
            };
        }

        #endregion
    }
}
