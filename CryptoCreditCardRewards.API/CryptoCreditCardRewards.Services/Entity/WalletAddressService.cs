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
    public class WalletAddressService : IWalletAddressService
    {
        private readonly CryptoCreditCardRewardsDbContext _context;
        private readonly WalletAddressSettings _walletAddressSettings;

        public WalletAddressService(CryptoCreditCardRewardsDbContext context, IOptions<WalletAddressSettings> walletAddressSettings)
        {
            _context = context;
            _walletAddressSettings = walletAddressSettings.Value;
        }

        /// <summary>
        /// Get a crypto currency wallet address for a user
        /// </summary>
        /// <param name="userId">The user to get the wallet address for</param>
        /// <param name="cryptoCurrencyId">The crypto currency to get the wallet address for</param>
        /// <param name="state">The state of the wallet address</param>
        /// <returns>A wallet address for currency/user</returns>
        public WalletAddress? GetUsersCurrencyWalletAddress(int userId, int cryptoCurrencyId, ActiveState state = ActiveState.Active)
        {
            // Get depo addresses for currency/user
            var addresses = _context.WalletAddresses.Where(x => x.CryptoCurrencyId == cryptoCurrencyId)
                .Where(x => x.UserId == userId);

            // Filter by active state
            if (state == ActiveState.Active)
                addresses = addresses.Where(x => x.Active);
            else if (state == ActiveState.InActive)
                addresses = addresses.Where(x => !x.Active);

            // Filter by id
            return addresses.FirstOrDefault();
        }

        /// <summary>
        /// Checks if a user has an address setup for a crypto currency
        /// </summary>
        /// <param name="cryptoCurrencyId">The crypto currency to check for</param>
        /// <param name="userId">The user to check for</param>
        /// <returns>True if a user has an address for the provided crypto currency</returns>
        public bool DoesUserOwnCryptoAddress(int cryptoCurrencyId, int userId)
        {
            return _context.WalletAddresses.Where(x => x.CryptoCurrencyId == cryptoCurrencyId)
                .Where(x => x.UserId == userId)
                .Any();
        }

        /// <summary>
        /// Get wallet addresses paged
        /// </summary>
        /// <param name="search">A search term (address/currency etc.)</param>
        /// <param name="userId">The user the wallets are for</param>
        /// <param name="cryptoCurrencyId">The crypto currency the wallets are for</param>
        /// <param name="page">The page to return</param>
        /// <param name="sortOrder">The order returned in</param>
        /// <returns>Paged wallet addresses</returns>
        public PagedResults<WalletAddress> GetWalletAddressesPaged(string? search, int? userId, int? cryptoCurrencyId, ActiveState? state, Page page, SortOrder sortOrder)
        {
            var walletAddresses = GetWalletAddresses(search, userId, cryptoCurrencyId, state);

            // Sort 
            walletAddresses = OrderWalletAddresses(walletAddresses, sortOrder);

            // Paginate
            var results = walletAddresses.Skip((int)(page.PageIndex * page.PerPage))
                .Take((int)page.PerPage)
                .ToList();

            // Get total
            var total = GetWalletAddresses(search, userId, cryptoCurrencyId, state).Select(x => x.Id).Count();

            // Map and return
            return new PagedResults<WalletAddress>()
            {
                Items = results,
                Page = page,
                SortOrder = sortOrder,
                TotalCount = total
            };
        }

        /// <summary>
        /// Get wallet addresses for a user/crypto currencies
        /// </summary>
        /// <param name="userId">The user to get wallet addresses for</param>
        /// <param name="cryptoCurrencyIds">The crypto currencies to get wallet addresses for</param>
        /// <returns>Wallet addresses</returns>
        public List<WalletAddress> GetWalletAddresses(int userId, List<int> cryptoCurrencyIds)
        {
            return _context.WalletAddresses.Where(x => x.UserId == userId)
                .Where(x => cryptoCurrencyIds.Contains(x.CryptoCurrencyId))
                .ToList();
        }

        /// <summary>
        /// Get a wallet address by its id
        /// </summary>
        /// <param name="walletAddressId">The wallet address to get</param>
        /// <returns>A wallet address</returns>
        public WalletAddress? GetWalletAddress(int walletAddressId, ActiveState state = ActiveState.Active)
        {
            // Filter by id
            var walletAddresses = _context.WalletAddresses.Include(x => x.CryptoCurrency)
                .Where(x => x.Id == walletAddressId);

            // Filter by active
            if (state == ActiveState.Active)
                walletAddresses = walletAddresses.Where(x => x.Active);
            else if (state == ActiveState.InActive)
                walletAddresses = walletAddresses.Where(x => !x.Active);

            return walletAddresses.FirstOrDefault();
        }

        /// <summary>
        /// Get a wallet address by its id
        /// </summary>
        /// <param name="walletAddressId">The wallet address to get</param>
        /// <returns>A wallet address</returns>
        public WalletAddress? GetWalletAddressById(int walletAddressId)
        {
            return _context.WalletAddresses.FirstOrDefault(x => x.Id == walletAddressId);
        }

        /// <summary>
        /// Create a wallet address for a user/currency
        /// </summary>
        /// <param name="userId">The user to create wallet address for</param>
        /// <param name="cryptoCurrencyId">The currency to create the wallet address for</param>
        /// <returns>A new wallet address</returns>
        public async Task<WalletAddress> CreateWalletAddressAsync(int userId, int cryptoCurrencyId)
        {
            // Get the currency
            var cryptoCurrency = _context.CryptoCurrencies.FirstOrDefault(x => x.Id == cryptoCurrencyId);

            // Check which key type to use to generate
            KeyData? keyData = null;
            switch (cryptoCurrency.InfrastructureType)
            {
                case InfrastructureType.EthereumRpc:
                    keyData = EthereumAddressUtility.GenerateAccount(_walletAddressSettings.Password);
                    break;
                case InfrastructureType.BitcoinQbitNinja:
                case InfrastructureType.BitcoinRpc:
                    keyData = BitcoinAddressUtility.GenerateAccount(_walletAddressSettings.Password, cryptoCurrency.IsTestNetwork);
                    break;
                default: throw new NotSupportedException($"AddressGenerationType {cryptoCurrency.InfrastructureType} is not supported");
            }

            // Build and save
            var walletAddress = new WalletAddress(true, keyData.PublicKey, keyData.PrivateData, cryptoCurrencyId, userId);

            _context.WalletAddresses.Add(walletAddress);
            await _context.SaveChangesAsync();

            return walletAddress;
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
                new ("cryptoCurrencyId", Order.Ascending),
                new ("cryptoCurrencyId", Order.Descending),
                new ("userId", Order.Ascending),
                new ("userId", Order.Descending),
            };
        }

        #region Helpers

        private IQueryable<WalletAddress> GetWalletAddresses(string? search, int? userId, int? cryptoCurrencyId, ActiveState? state)
        {
            // Get addresses with currency and user
            var walletAddresses = _context.WalletAddresses.Include(x => x.CryptoCurrency)
                .Include(x => x.User)
                .AsQueryable();

            // Filter by search term
            if(!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().Trim();
                walletAddresses = walletAddresses.Where(x => (x.CryptoCurrency != null && x.CryptoCurrency.Name.ToLower().Contains(search)) ||
                (x.Address.ToLower().Contains(search)));
            }

            // Filter by user
            if(userId.HasValue)
            {
                walletAddresses = walletAddresses.Where(x => x.UserId == userId);
            }

            // Filter by currency
            if (cryptoCurrencyId.HasValue)
            {
                walletAddresses = walletAddresses.Where(x => x.CryptoCurrencyId == cryptoCurrencyId);
            }

            // Filter by active state
            if(state == ActiveState.Active)
            {
                walletAddresses = walletAddresses.Where(x => x.Active);
            }
            else if (state == ActiveState.InActive)
            {
                walletAddresses = walletAddresses.Where(x => !x.Active);
            }

            return walletAddresses;
        }

        /// <summary>
        /// Orders wallet addresses in a queryable list
        /// </summary>
        /// <param name="users">The list to order</param>
        /// <param name="sortOrder">The sort order details</param>
        /// <returns>Sorted wallet address list</returns>
        private IQueryable<WalletAddress> OrderWalletAddresses(IQueryable<WalletAddress> walletAddresses, SortOrder sortOrder)
        {
            return (sortOrder.OrderProperty.Trim(), sortOrder.Order) switch
            {
                ("createdDate", Order.Ascending) => walletAddresses.OrderBy(x => x.CreatedDate),
                ("createdDate", Order.Descending) => walletAddresses.OrderByDescending(x => x.CreatedDate),
                ("cryptoCurrencyId", Order.Ascending) => walletAddresses.OrderBy(x => x.CryptoCurrencyId),
                ("cryptoCurrencyId", Order.Descending) => walletAddresses.OrderByDescending(x => x.CryptoCurrencyId),
                ("userId", Order.Ascending) => walletAddresses.OrderBy(x => x.UserId),
                ("userId", Order.Descending) => walletAddresses.OrderByDescending(x => x.UserId),
                _ => sortOrder.Order == Order.Ascending ? walletAddresses.OrderBy(x => x.CreatedDate) : walletAddresses.OrderByDescending(x => x.CreatedDate)
            };
        }

        #endregion
    }
}
