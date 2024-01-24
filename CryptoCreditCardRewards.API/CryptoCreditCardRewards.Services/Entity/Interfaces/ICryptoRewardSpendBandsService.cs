using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos.CreditCardTransactions;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Services.Entity.Interfaces
{
    public interface ICryptoRewardBandsService
    {
        /// <summary>
        /// Get the amount to reward a user with based on existing spend bands
        /// </summary>
        /// <param name="aggregateSpendAmount">The amount spent</param>
        /// <param name="aggregateStakeAmount">The amount staked</param>
        /// <returns>The amount to reward user based on spend</returns>
        decimal GetRewardTotal(decimal aggregateSpendAmount, decimal aggregateStakeAmount);

        /// <summary>
        /// Get a reward spend band
        /// </summary>
        /// <param name="id">The reward spend band to get</param>
        /// <param name="state">The state of the reward spend band</param>
        /// <returns>A reward spend band</returns>
        CryptoRewardSpendBand? GetRewardBand(int id, ActiveState state);

        /// <summary>
        /// Delete a reward band
        /// </summary>
        /// <param name="id">The reward band to delete</param>
        /// <returns>An async task</returns>
        Task DeleteRewardBandAsync(int id);

        /// <summary>
        /// Gets the properties to sort by
        /// </summary>
        /// <returns>All properties to sort by</returns>
        List<(string PropertyName, Order Order)> GetSortProperties();

        /// <param name="search">Name/description</param>
        /// <param name="bandType">The type of band</param>
        /// <param name="fromBandFrom">From band from value</param>
        /// <param name="toBandFrom">To band from value</param>
        /// <param name="fromBandTo">From band to value</param>
        /// <param name="toBandTo">To band to value</param>
        /// <param name="fromPercentageReward">From percentage reward value</param>
        /// <param name="toPercentageReward">To percentage reward value</param>
        /// <param name="fromUpTo">From up to value</param>
        /// <param name="toUpTo">To up to value</param>
        /// <param name="sortOrder">The order to return the results</param>
        /// <param name="page">The page to be returned</param>
        PagedResults<CryptoRewardSpendBand> GetCryptoRewardSpendBandsPaged(string? search, decimal? fromBandFrom, decimal? fromBandTo, decimal? toBandFrom, decimal? toBandTo, 
            decimal? fromPercentageReward, decimal? toPercentageReward, decimal? fromUpTo, decimal? toUpTo, BandType? bandType, Page page, SortOrder sortOrder);

        /// <summary>
        /// See if the range specified crosses an existing range for a band type
        /// </summary>
        /// <param name="bandType">The type of band to check</param>
        /// <param name="bandFrom">The band from range</param>
        /// <param name="bandTo">The band to range</param>
        /// <returns>True if the range specified conflicts with an existing band</returns>
        bool DoesRangeCrossAnotherBand(BandType bandType, decimal bandFrom, decimal bandTo);

        /// <summary>
        /// Create a reward spend band
        /// </summary>
        /// <param name="bandType">The type of band to create</param>
        /// <param name="name">The bands name</param>
        /// <param name="description">A descreiption/notes for the band</param>
        /// <param name="bandFrom">The band value from</param>
        /// <param name="bandTo">The band value to</param>
        /// <param name="percentageReward">The amount reward in %</param>
        /// <returns>The reward spend band created</returns>
        Task<CryptoRewardSpendBand> CreateCryptoRewardSpendBandAsync(BandType bandType, string? name, string? description, decimal bandFrom, decimal bandTo, decimal percentageReward);
    }
}