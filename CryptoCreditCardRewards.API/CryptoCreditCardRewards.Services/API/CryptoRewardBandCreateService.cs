using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.Services.API
{
    public class CryptoRewardBandCreateService : ICryptoRewardBandCreateService
    {
        private readonly ICryptoRewardBandsService _cryptoRewardBandService;
        private readonly IWalletAddressService _walletAddressService;

        public CryptoRewardBandCreateService(ICryptoRewardBandsService cryptoRewardBandService, IWalletAddressService walletAddressService)
        {
            _cryptoRewardBandService = cryptoRewardBandService;
            _walletAddressService = walletAddressService;
        }

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
        public async Task<CryptoRewardSpendBand> CreateRewardSpendBandAsync(BandType bandType, string? name, string? description, decimal bandFrom, decimal bandTo, decimal percentageReward, int? cryptoCurrencyId)
        {
            // Ensure band from < band to
            if (bandFrom > bandTo)
                throw new BadRequestException(FailedReason.BandToMustBeGreaterThanBandFrom, Property.BandTo);

            // Ensure bandTo is greater than 0
            if (bandTo <= 0)
                throw new BadRequestException(FailedReason.BandToMustBeGreaterThan0, Property.BandTo);

            // Ensure the percentage reward is +ve
            if (percentageReward <= 0)
                throw new BadRequestException(FailedReason.PercentageRewardMustBeGreaterThan0, Property.PercentageReward);

            // Check if band crosses another existing
            var doesBandCrossAnother = _cryptoRewardBandService.DoesRangeCrossAnotherBand(bandType, bandFrom, bandTo);
            if (doesBandCrossAnother)
                throw new UnprocessableEntityException(FailedReason.RangeCrossesAnExistingRange, Property.BandTo);

            // Create a crypto reward band
            return await _cryptoRewardBandService.CreateCryptoRewardSpendBandAsync(bandType, name, description, bandFrom, bandTo, percentageReward);
        }
    }
}
