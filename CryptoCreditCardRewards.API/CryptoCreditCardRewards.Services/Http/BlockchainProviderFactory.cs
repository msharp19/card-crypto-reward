using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Http.Interfaces;
using CryptoCreditCardRewards.Services.Misc.Interfaces;

namespace CryptoCreditCardRewards.Services.Http
{
    public class ConversionProviderFactory : IConversionProviderFactory
    {
        private readonly ICacheService _cacheService;
        private readonly IServiceProvider _serviceProvider;

        public ConversionProviderFactory(IServiceProvider serviceProvider, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets a conversion service
        /// </summary>
        /// <param name="conversionServiceType">The type of service to return</param>
        /// <returns>A conversion service if supported</returns>
        public IConversionService GetConversionService(ConversionServiceType conversionServiceType)
        {
            switch (conversionServiceType)
            {
                case ConversionServiceType.CryptoCompare: return (IConversionService) _serviceProvider.GetService(typeof(CryptoCompareService));
                default: throw new NotSupportedException($"Conversion service type: {conversionServiceType} is not yet supported.");
            };
        }
    }
}
