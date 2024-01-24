using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.Services.API
{
    public class CryptoRewardBandDeleteService : ICryptoRewardBandDeleteService
    {
        private readonly ICryptoRewardBandsService _cryptoRewardBandService;

        public CryptoRewardBandDeleteService(ICryptoRewardBandsService cryptoRewardBandService)
        {
            _cryptoRewardBandService = cryptoRewardBandService;
        }

        /// <summary>
        /// Delete a reward band
        /// </summary>
        /// <param name="id">The reward band to delete</param>
        /// <returns>An async task</returns>
        public async Task DeleteRewardBandAsync(int id)
        {
            // Get the reward spend band
            var rewardSpendBand = _cryptoRewardBandService.GetRewardBand(id, ActiveState.Both);
            if (rewardSpendBand == null)
                throw new NotFoundException(FailedReason.RewardSpendBandDoesntExist, Property.Id);

            await _cryptoRewardBandService.DeleteRewardBandAsync(id);
        }
    }
}
