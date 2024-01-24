using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.Api.Interfaces;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Http.Interfaces;

namespace CryptoCreditCardRewards.Services.Api
{
    public class CryptoCurrencyCreateService : ICryptoCurrencyCreateService
    {
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly IBlockchainProviderFactory _blockchainServiceProviderFactory;
        private readonly IConversionProviderFactory _conversionProviderrFactory;

        public CryptoCurrencyCreateService(ICryptoCurrencyService cryptoCurrencyService, IBlockchainProviderFactory blockchainServiceProviderFactory,
            IConversionProviderFactory conversionProviderrFactory)
        {
            _cryptoCurrencyService = cryptoCurrencyService;
            _blockchainServiceProviderFactory = blockchainServiceProviderFactory;
            _conversionProviderrFactory = conversionProviderrFactory;
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
        public async Task<CryptoCurrency> CreateCryptoCurrencyAsync(string name, string symbol, string? description, bool active, string networkEndpoint, bool isTestNetwork, 
            bool supportsStaking, InfrastructureType infrastructureType, ConversionServiceType conversionServiceType)
        {
            // Get the blockchain service
            var blockChainService = _blockchainServiceProviderFactory.GetBlockchainService(infrastructureType, isTestNetwork ? NetworkType.Test : NetworkType.Main,
                networkEndpoint);

            // Try to validate network
            var networkValidated = await blockChainService.ValidateNetworkAsync(isTestNetwork);
            if (!networkValidated)
                throw new UnprocessableEntityException(FailedReason.NetworkIsntValid, Property.NetworkEndpoint);

            // Try to validate conversion
            var conversionService = _conversionProviderrFactory.GetConversionService(conversionServiceType);
            var conversionValidated = await conversionService.IsSupportedAsync(symbol?.Trim()?.ToUpper());
            if (!conversionValidated)
                throw new UnprocessableEntityException(FailedReason.SymbolNotSupportedForConversionImplementation, Property.ConversionServiceType);

            // Create
            return await _cryptoCurrencyService.CreateCryptoCurrencyAsync(name, symbol?.Trim()?.ToUpper(), description, active, networkEndpoint, isTestNetwork, supportsStaking, infrastructureType, conversionServiceType);
        }
    }
}
