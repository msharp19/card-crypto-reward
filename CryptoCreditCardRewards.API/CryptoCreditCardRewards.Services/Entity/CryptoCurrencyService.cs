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
    public class CryptoCurrencyService : ICryptoCurrencyService
    {
        private readonly CryptoCreditCardRewardsDbContext _context;

        public CryptoCurrencyService(CryptoCreditCardRewardsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create a new crypto currency
        /// </summary>
        /// <param name="name">The name of the currency</param>
        /// <param name="symbol">A symbol for the currency</param>
        /// <param name="description">A description for the currency</param>
        /// <param name="active">If the currency is active in the system</param>
        /// <param name="networkEndpoint">The endpoint to contact network for transactions</param>
        /// <param name="isTestNetwork">True if is a test network</param>
        /// <param name="supportsStaking">True if we support the crypto currency for staking</param>
        /// <param name="infrastructureType">The type of infrastructure the crypto currency works on</param>
        /// <returns>The new crypto currency</returns>
        public async Task<CryptoCurrency> CreateCryptoCurrencyAsync(string name, string symbol, string? description, bool active, string networkEndpoint, bool isTestNetwork, bool supportsStaking,
            InfrastructureType infrastructureType, ConversionServiceType conversionServiceType)
        {
            var newCryptoCurrency = new CryptoCurrency(active, name, networkEndpoint, symbol, isTestNetwork, description, supportsStaking, infrastructureType, conversionServiceType);

            _context.CryptoCurrencies.Add(newCryptoCurrency);
            await _context.SaveChangesAsync();

            return newCryptoCurrency;
        }

        /// <summary>
        /// Get a list of paged currencies
        /// </summary>
        /// <param name="search">Searches name of currency & symbol</param>
        /// <param name="symbol">The symbol of the currency</param>
        /// <param name="stakable">If the currency is stakable or not</param>
        /// <param name="state">The currenct state of the currencies</param>
        /// <param name="page">The page to return</param>
        /// <param name="sortOrder">The order to return</param>
        /// <returns>A page of currencies</returns>
        public PagedResults<CryptoCurrency> GetCurrenciesPaged(string? search, string? symbol,bool? stakable, ActiveState state, Page page, SortOrder sortOrder)
        {
            // Get currencies query
            var users = GetCurrenciesQuery(search, symbol, stakable, state);

            // Sort 
            users = OrderCurrencies(users, sortOrder);

            // Paginate
            var results = users.Skip((int)(page.PageIndex * page.PerPage))
                .Take((int)page.PerPage)
                .ToList();

            // Get total
            var total = GetCurrenciesQuery(search, symbol, stakable, state).Select(x => x.Id).Count();

            // Map and return
            return new PagedResults<CryptoCurrency>()
            {
                Items = results,
                Page = page,
                SortOrder = sortOrder,
                TotalCount = total
            };
        }

        /// <summary>
        /// Get a crypto currency by its id
        /// </summary>
        /// <param name="cryptoCurrencyId">The id to get the crypto currency by</param>
        /// <param name="state">The state of the currency</param>
        /// <returns>A crypto currency if it exists</returns>
        public CryptoCurrency? GetCryptoCurrency(int cryptoCurrencyId, ActiveState state = ActiveState.Active)
        {
            // Get currencies
            var currencies = _context.CryptoCurrencies.AsQueryable();

            // Filter by active state
            if (state == ActiveState.Active)
                currencies = currencies.Where(x => x.Active);
            else if (state == ActiveState.InActive)
                currencies = currencies.Where(x => !x.Active);

            // Filter by id
            return currencies.FirstOrDefault(x => x.Id == cryptoCurrencyId);
        }

        /// <summary>
        /// Get crypto currencies by their ids
        /// </summary>
        /// <param name="cryptoCurrencyIds">The crypto currencies to get</param>
        /// <returns>Crypto currencies</returns>
        public List<CryptoCurrency> GetCryptoCurrenciesByIds(List<int> cryptoCurrencyIds)
        {
            return _context.CryptoCurrencies.Where(x => cryptoCurrencyIds.Contains(x.Id))
                .ToList();
        }

        /// <summary>
        /// Update a crypto currencies state
        /// </summary>
        /// <param name="cryptoCurrencyId">The crypto currency to update state of</param>
        /// <param name="state">The new state</param>
        /// <returns>The updated currency</returns>
        public async Task<CryptoCurrency> UpdateCryptoCurrencyStateAsync(int cryptoCurrencyId, bool state)
        {
            var cryptoCurrency = GetCryptoCurrency(cryptoCurrencyId, ActiveState.Both);

            cryptoCurrency.SetActiveState(state);

            _context.CryptoCurrencies.Update(cryptoCurrency);
            await _context.SaveChangesAsync();

            return cryptoCurrency;
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
                new ("name", Order.Ascending),
                new ("name", Order.Descending),
                new ("symbol", Order.Ascending),
                new ("symbol", Order.Descending),
            };
        }

        #region Helpers

        private IQueryable<CryptoCurrency> GetCurrenciesQuery(string? search, string? symbol, bool? stakable, ActiveState state)
        {
            var cryptoCurrencies = _context.CryptoCurrencies.AsQueryable();

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearchTerm = search.ToLower().Trim();

                cryptoCurrencies = cryptoCurrencies.Where(x => x.Symbol.ToLower().Contains(lowerSearchTerm) ||
                     x.Name.ToLower().Contains(lowerSearchTerm));
            }

            // Filter by symbol
            if(!string.IsNullOrEmpty(symbol))
            {
                cryptoCurrencies = cryptoCurrencies.Where(x => x.Symbol == symbol);
            }

            // Filter by stakability
            if (stakable.HasValue)
            {
                cryptoCurrencies = cryptoCurrencies.Where(x => x.SupportsStaking == stakable.Value);
            }

            // Filter by active state
            if(state == ActiveState.Active)
            {
                cryptoCurrencies = cryptoCurrencies.Where(x => x.Active);
            }
            else if (state == ActiveState.InActive)
            {
                cryptoCurrencies = cryptoCurrencies.Where(x => !x.Active);
            }

            return cryptoCurrencies;
        }

        /// <summary>
        /// Orders crypto currencies in a queryable list
        /// </summary>
        /// <param name="cryptocurrencies">The list to order</param>
        /// <param name="sortOrder">The sort order details</param>
        /// <returns>Sorted user list</returns>
        private IQueryable<CryptoCurrency> OrderCurrencies(IQueryable<CryptoCurrency> cryptocurrencies, SortOrder sortOrder)
        {
            // Sort users where supported property exists - default name
            return (sortOrder.OrderProperty.Trim(), sortOrder.Order) switch
            {
                ("createdDate", Order.Ascending) => cryptocurrencies.OrderBy(x => x.CreatedDate),
                ("createdDate", Order.Descending) => cryptocurrencies.OrderByDescending(x => x.CreatedDate),
                ("name", Order.Ascending) => cryptocurrencies.OrderBy(x => x.Name),
                ("name", Order.Descending) => cryptocurrencies.OrderByDescending(x => x.Name),
                ("symbol", Order.Ascending) => cryptocurrencies.OrderBy(x => x.Symbol),
                ("symbol", Order.Descending) => cryptocurrencies.OrderByDescending(x => x.Symbol),
                _ => sortOrder.Order == Order.Ascending ? cryptocurrencies.OrderBy(x => x.CreatedDate) : cryptocurrencies.OrderByDescending(x => x.CreatedDate)
            };
        }

        #endregion
    }
}
