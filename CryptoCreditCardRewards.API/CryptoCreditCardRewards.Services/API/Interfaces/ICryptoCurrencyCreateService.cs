using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Services.Api.Interfaces
{
    public interface ICryptoCurrencyCreateService
    {
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
