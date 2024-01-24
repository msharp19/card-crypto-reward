using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Contexts;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Utilities;
using CryptoCreditCardRewards.Utilities.Models;

namespace CryptoCreditCardRewards.Services.Entity
{
    public class SystemWalletAddressService : ISystemWalletAddressService
    {
        private readonly CryptoCreditCardRewardsDbContext _context;
        private readonly SystemWalletAddressSettings _systemWalletAddressSettings;

        public SystemWalletAddressService(CryptoCreditCardRewardsDbContext context, IOptions<SystemWalletAddressSettings> systemWalletAddressSettings)
        {
            _context = context;
            _systemWalletAddressSettings = systemWalletAddressSettings.Value;
        }

        /// <summary>
        /// Create a system wallet address for a currency
        /// </summary>
        /// <param name="cryptoCurrencyId">The currency to create the wallet address for</param>
        /// <param name="addressType">The address type</param>
        /// <returns>A new wallet address</returns>
        public async Task<SystemWalletAddress> CreateSystemWalletAddressAsync(int cryptoCurrencyId, AddressType addressType)
        {
            // Get the currency
            var cryptoCurrency = _context.CryptoCurrencies.FirstOrDefault(x => x.Id == cryptoCurrencyId);

            // Check which key type to use to generate
            KeyData? keyData = null;
            switch (cryptoCurrency.InfrastructureType)
            {
                case InfrastructureType.EthereumRpc:
                    keyData = EthereumAddressUtility.GenerateAccount(_systemWalletAddressSettings.Password);
                    break;
                case InfrastructureType.BitcoinQbitNinja:
                case InfrastructureType.BitcoinRpc:
                    keyData = BitcoinAddressUtility.GenerateAccount(_systemWalletAddressSettings.Password, cryptoCurrency.IsTestNetwork);
                    break;
                default: throw new NotSupportedException($"AddressGenerationType {cryptoCurrency.InfrastructureType} is not supported");
            }

            // Build and save
            var walletAddress = new SystemWalletAddress(true, addressType, keyData.PublicKey, keyData.PrivateData, cryptoCurrencyId);

            _context.SystemWalletAddresses.Add(walletAddress);
            await _context.SaveChangesAsync();

            return walletAddress;
        }

        /// <summary>
        /// Get a system crypto currency wallet address
        /// </summary>
        /// <param name="cryptoCurrencyId">The crypto currency to get the wallet address for</param>
        /// <param name="state">The state of the wallet address</param>
        /// <returns>A wallet address for currency/user</returns>
        public SystemWalletAddress? GetSystemWalletAddress(int cryptoCurrencyId, AddressType addressType, ActiveState state = ActiveState.Active)
        {
            // Filter by id & address type
            var walletAddresses = _context.SystemWalletAddresses.Where(x => x.CryptoCurrencyId == cryptoCurrencyId)
                .Where(x => x.AddressType == addressType);

            // Filter by active
            if (state == ActiveState.Active)
                walletAddresses = walletAddresses.Where(x => x.Active);
            else if (state == ActiveState.InActive)
                walletAddresses = walletAddresses.Where(x => !x.Active);

            return walletAddresses.FirstOrDefault();
        }

        /// <summary>
        /// Get a system wallet address by its id
        /// </summary>
        /// <param name="walletAddressId">The wallet address to get</param>
        /// <returns>A wallet address</returns>
        public SystemWalletAddress? GetSystemWalletAddressById(int walletAddressId, ActiveState state = ActiveState.Active)
        {
            // Filter by id
            var walletAddresses = _context.SystemWalletAddresses.Where(x => x.Id == walletAddressId);

            // Filter by active
            if (state == ActiveState.Active)
                walletAddresses = walletAddresses.Where(x => x.Active);
            else if (state == ActiveState.InActive)
                walletAddresses = walletAddresses.Where(x => !x.Active);

            return walletAddresses.FirstOrDefault();
        }

        /// <summary>
        /// Get system wallet addresses paged
        /// </summary>
        /// <param name="search">A search term (address/currency etc.)</param>
        /// <param name="cryptoCurrencyId">The crypto currency the wallets are for</param>
        /// <param name="page">The page to return</param>
        /// <param name="sortOrder">The order returned in</param>
        /// <returns>Paged system wallet addresses</returns>
        public PagedResults<SystemWalletAddress> GetSystemWalletAddressesPaged(string? search, int? cryptoCurrencyId, ActiveState? state, Page page, SortOrder sortOrder)
        {
            var systemWalletAddresses = GetSystemWalletAddresses(search, cryptoCurrencyId, state);

            // Sort 
            systemWalletAddresses = OrderSystemWalletAddresses(systemWalletAddresses, sortOrder);

            // Paginate
            var results = systemWalletAddresses.Skip((int)(page.PageIndex * page.PerPage))
                .Take((int)page.PerPage)
                .ToList();

            // Get total
            var total = GetSystemWalletAddresses(search, cryptoCurrencyId, state).Select(x => x.Id).Count();

            // Map and return
            return new PagedResults<SystemWalletAddress>()
            {
                Items = results,
                Page = page,
                SortOrder = sortOrder,
                TotalCount = total
            };
        }

        /// <summary>
        /// Get system wallet addresses for address type/currencies
        /// </summary>
        /// <param name="cryptoCurrencyIds">The currencies to get system wallets for</param>
        /// <param name="addressType">The address type</param>
        /// <param name="state">If the wallet addresses are active or not</param>
        /// <returns>System wallet addresses</returns>
        public List<SystemWalletAddress> GetSystemWalletAddresses(List<int> cryptoCurrencyIds, AddressType addressType, ActiveState state)
        {
            var addresses = _context.SystemWalletAddresses.Where(x => x.AddressType == addressType)
                .Where(x => cryptoCurrencyIds.Contains(x.CryptoCurrencyId));

            // Filter by active
            if (state == ActiveState.Active)
                addresses = addresses.Where(x => x.Active);
            else if (state == ActiveState.InActive)
                addresses = addresses.Where(x => !x.Active);

            return addresses.ToList();
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
            };
        }

        #region Helpers

        private IQueryable<SystemWalletAddress> GetSystemWalletAddresses(string? search, int? cryptoCurrencyId, ActiveState? state)
        {
            // Get addresses with currency and user
            var systemWalletAddresses = _context.SystemWalletAddresses.Include(x => x.CryptoCurrency)
                .AsQueryable();

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().Trim();
                systemWalletAddresses = systemWalletAddresses.Where(x => (x.CryptoCurrency != null && x.CryptoCurrency.Name.ToLower().Contains(search)) ||
                (x.Address.ToLower().Contains(search)));
            }

            // Filter by currency
            if (cryptoCurrencyId.HasValue)
            {
                systemWalletAddresses = systemWalletAddresses.Where(x => x.CryptoCurrencyId == cryptoCurrencyId);
            }

            // Filter by active state
            if (state == ActiveState.Active)
            {
                systemWalletAddresses = systemWalletAddresses.Where(x => x.Active);
            }
            else if (state == ActiveState.InActive)
            {
                systemWalletAddresses = systemWalletAddresses.Where(x => !x.Active);
            }

            return systemWalletAddresses;
        }

        /// <summary>
        /// Orders wallet addresses in a queryable list
        /// </summary>
        /// <param name="users">The list to order</param>
        /// <param name="sortOrder">The sort order details</param>
        /// <returns>Sorted wallet address list</returns>
        private IQueryable<SystemWalletAddress> OrderSystemWalletAddresses(IQueryable<SystemWalletAddress> walletAddresses, SortOrder sortOrder)
        {
            return (sortOrder.OrderProperty.Trim(), sortOrder.Order) switch
            {
                ("createdDate", Order.Ascending) => walletAddresses.OrderBy(x => x.CreatedDate),
                ("createdDate", Order.Descending) => walletAddresses.OrderByDescending(x => x.CreatedDate),
                _ => sortOrder.Order == Order.Ascending ? walletAddresses.OrderBy(x => x.CreatedDate) : walletAddresses.OrderByDescending(x => x.CreatedDate)
            };
        }

        #endregion
    }
}
