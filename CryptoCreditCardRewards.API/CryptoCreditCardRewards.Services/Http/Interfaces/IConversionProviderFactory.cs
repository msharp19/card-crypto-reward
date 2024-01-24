using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Services.Http.Interfaces
{
    public interface IConversionProviderFactory
    {
        /// <summary>
        /// Gets a conversion service
        /// </summary>
        /// <param name="conversionServiceType">The type of service to return</param>
        /// <returns>A conversion service if supported</returns>
        IConversionService GetConversionService(ConversionServiceType conversionServiceType);
    }
}
