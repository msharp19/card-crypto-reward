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
    public interface ICryptoCurrencyService
    {
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
        PagedResults<CryptoCurrency> GetCurrenciesPaged(string? search, string? symbol, bool? stakable, ActiveState state, Page page, SortOrder sortOrder);

        /// <summary>
        /// Update a crypto currencies state
        /// </summary>
        /// <param name="cryptoCurrencyId">The crypto currency to update state of</param>
        /// <param name="state">The new state</param>
        /// <returns>The updated currency</returns>
        Task<CryptoCurrency> UpdateCryptoCurrencyStateAsync(int cryptoCurrencyId, bool state);

        /// <summary>
        /// Get a crypto currency by its id
        /// </summary>
        /// <param name="cryptoCurrencyId">The id to get the crypto currency by</param>
        /// <param name="state">The state of the currency</param>
        /// <returns>A crypto currency if it exists</returns>
        CryptoCurrency? GetCryptoCurrency(int cryptoCurrencyId, ActiveState state = ActiveState.Active);

        /// <summary>
        /// Gets the properties to sort by
        /// </summary>
        /// <returns>All properties to sort by</returns>
        List<(string PropertyName, Order Order)> GetSortProperties();

        /// <summary>
        /// Get crypto currencies by their ids
        /// </summary>
        /// <param name="cryptoCurrencyIds">The crypto currencies to get</param>
        /// <returns>Crypto currencies</returns>
        List<CryptoCurrency> GetCryptoCurrenciesByIds(List<int> cryptoCurrencyIds);

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
        Task<CryptoCurrency> CreateCryptoCurrencyAsync(string name, string symbol, string? description, bool active, string networkEndpoint, bool isTestNetwork, bool supportsStaking, 
            InfrastructureType infrastructureType, ConversionServiceType conversionServiceType);
    }
}
