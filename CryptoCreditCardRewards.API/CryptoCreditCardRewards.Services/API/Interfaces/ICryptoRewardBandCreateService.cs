using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface ICryptoRewardBandCreateService
    {
        /// <summary>
        /// Create a reward spend band
        /// </summary>
        /// <param name="bandType">The type of band to create</param>
        /// <param name="name">The bands name</param>
        /// <param name="description">A descreiption/notes for the band</param>
        /// <param name="bandFrom">The band value from</param>
        /// <param name="bandTo">The band value to</param>
        /// <param name="percentageReward">The amount reward in %</param>
        /// <returns>The new reward band</returns>
        /// <exception cref="BadRequestException">Thrown if
        ///       - BandFrom > band to
        ///       - BandTo <= 0
        ///       - Percentage reward <= 0
        /// </exception>
        /// <exception cref="UnprocessableEntityException">Thrown if
        ///       - Band range crosses an existing band with the sames types range
        /// </exception>
        Task<CryptoRewardSpendBand> CreateRewardSpendBandAsync(BandType bandType, string? name, string? description, decimal bandFrom, decimal bandTo, decimal percentageReward, int? cryptoCurrencyId);
    }
}
