using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Contexts;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos.CreditCardTransactions;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.Services.Entity
{
    public class CryptoRewardBandsService : ICryptoRewardBandsService
    {
        private readonly CryptoCreditCardRewardsDbContext _context;

        public CryptoRewardBandsService(CryptoCreditCardRewardsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the amount to reward a user with based on existing spend bands
        /// </summary>
        /// <param name="aggregateSpendAmount">The amount spent</param>
        /// <param name="aggregateStakeAmount">The amount staked</param>
        /// <returns>The amount to reward user based on spend</returns>
        public decimal GetRewardTotal(decimal aggregateSpendAmount, decimal aggregateStakeAmount)
        {
            // TODO: if to support multi currency in future, will need a conversion here.
            // AggregateValue contains the currency it is in. Master currency we are working with is HKD
            var spendBand = _context.CryptoRewardSpendBands.Where(x => x.Active)
                .Where(x => x.BandFrom <= aggregateSpendAmount)
                .Where(x => x.BandTo >= aggregateSpendAmount)
                .Where(x => x.Type == BandType.Spend)
                .FirstOrDefault();

            // If the user has spent more than the bands cover, then we choose the highest band for spend
            if (spendBand == null)
            {
                spendBand = _context.CryptoRewardSpendBands.Where(x => x.Active)
                .Where(x => x.Type == BandType.Spend)
                .OrderByDescending(x => x.BandTo)
                .FirstOrDefault();
            }

            var stakeBand = _context.CryptoRewardSpendBands.Where(x => x.Active)
              .Where(x => x.BandFrom >= aggregateStakeAmount)
              .Where(x => aggregateStakeAmount <= x.BandTo)
              .Where(x => x.Type == BandType.Stake)
              .FirstOrDefault();

            // Get the percentage to add to reward for spend & stake
            var spendRewardPercent = (spendBand?.PercentageReward ?? 0m);
            var stakeRewardPercent = (stakeBand?.PercentageReward ?? 0m);

            // Get the maximum amount of spend/stake to give percent on 
            var spendRewardTo = (spendBand?.UpTo ?? 0m);
            var stakeRewardTo = (stakeBand?.UpTo ?? 0m);

            // Get the value to get the percentage of (maximum or value if less than maximum)
            var spendRewardValueToGetPercentageOf = aggregateSpendAmount > spendRewardTo ? spendRewardTo : aggregateSpendAmount;
            var stakeRewardValueToGetPercentageOf = aggregateStakeAmount > stakeRewardTo ? stakeRewardTo : aggregateStakeAmount;

            // Return the % of spend & staking summed together (the reward)
            return ((spendRewardValueToGetPercentageOf / 100m) * spendRewardPercent) + ((stakeRewardValueToGetPercentageOf / 100m) * stakeRewardPercent);
        }

        /// <summary>
        /// Get a reward spend band
        /// </summary>
        /// <param name="id">The reward spend band to get</param>
        /// <param name="state">The state of the reward spend band</param>
        /// <returns>A reward spend band</returns>
        public CryptoRewardSpendBand? GetRewardBand(int id, ActiveState state)
        {
            // Get reward spend bands
            var rewardSpendBands = _context.CryptoRewardSpendBands.AsQueryable();

            // Filter by active state
            if (state == ActiveState.Active)
                rewardSpendBands = rewardSpendBands.Where(x => x.Active);
            else if (state == ActiveState.InActive)
                rewardSpendBands = rewardSpendBands.Where(x => !x.Active);

            // Filter by id
            return rewardSpendBands.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Delete a reward band
        /// </summary>
        /// <param name="id">The reward band to delete</param>
        /// <returns>An async task</returns>
        public async Task DeleteRewardBandAsync(int id)
        {
            var rewardBand = _context.CryptoRewardSpendBands.FirstOrDefault(x => x.Id == id);

            _context.CryptoRewardSpendBands.Remove(rewardBand);
            await _context.SaveChangesAsync();

            return;
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
                new ("bandFrom", Order.Ascending),
                new ("bandFrom", Order.Descending),
                new ("bandTo", Order.Ascending),
                new ("bandTo", Order.Descending),
                new ("percentageReward", Order.Ascending),
                new ("percentageReward", Order.Descending),
            };
        }

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
        public PagedResults<CryptoRewardSpendBand> GetCryptoRewardSpendBandsPaged(string? search, decimal? fromBandFrom, decimal? fromBandTo, decimal? toBandFrom, decimal? toBandTo,
            decimal? fromPercentageReward, decimal? toPercentageReward, decimal? fromUpTo, decimal? toUpTo, BandType? bandType, Page page, SortOrder sortOrder)
        {
            // Get reward spend bands query
            var cryptoRewardSpendBands = GetCryptoRewardSpendBandsQuery(search, fromBandFrom, fromBandTo, toBandFrom, toBandTo, fromPercentageReward, toPercentageReward, fromUpTo, toUpTo, bandType);

            // Sort 
            cryptoRewardSpendBands = OrderRewardSpendBands(cryptoRewardSpendBands, sortOrder);

            // Paginate
            var results = cryptoRewardSpendBands.Skip((int)(page.PageIndex * page.PerPage))
                .Take((int)page.PerPage)
                .ToList();

            // Get total
            var total = GetCryptoRewardSpendBandsQuery(search, fromBandFrom, fromBandTo, toBandFrom, toBandTo, fromPercentageReward, toPercentageReward, fromUpTo, toUpTo, bandType).Count();

            // Map and return
            return new PagedResults<CryptoRewardSpendBand>()
            {
                Items = results,
                Page = page,
                SortOrder = sortOrder,
                TotalCount = total
            };
        }

        /// <summary>
        /// See if the range specified crosses an existing range for a band type
        /// </summary>
        /// <param name="bandType">The type of band to check</param>
        /// <param name="bandFrom">The band from range</param>
        /// <param name="bandTo">The band to range</param>
        /// <returns>True if the range specified conflicts with an existing band</returns>
        public bool DoesRangeCrossAnotherBand(BandType bandType, decimal bandFrom, decimal bandTo)
        {
            return _context.CryptoRewardSpendBands.Any(x => x.BandFrom < bandFrom && bandTo < x.BandTo);
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
        /// <returns>The reward spend band created</returns>
        public async Task<CryptoRewardSpendBand> CreateCryptoRewardSpendBandAsync(BandType bandType, string? name, string? description, decimal bandFrom, decimal bandTo, decimal percentageReward)
        {
            var rewardSpendBand = new CryptoRewardSpendBand(true, name, description, bandFrom, bandTo, bandTo, percentageReward, bandType);

            _context.CryptoRewardSpendBands.Add(rewardSpendBand);
            await _context.SaveChangesAsync();

            return rewardSpendBand;
        }

        #region Helpers

        private IQueryable<CryptoRewardSpendBand> GetCryptoRewardSpendBandsQuery(string? search, decimal? fromBandFrom, decimal? fromBandTo, decimal? toBandFrom, decimal? toBandTo,
           decimal? fromPercentageReward, decimal? toPercentageReward, decimal? fromUpTo, decimal? toUpTo, BandType? bandType)
        {
            var rewardSpendBands = _context.CryptoRewardSpendBands.AsQueryable();

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearchTerm = search.ToLower().Trim();
                rewardSpendBands = rewardSpendBands.Where(x => (x.Name != null && x.Name.ToLower().Contains(lowerSearchTerm)) ||
                     (x.Description != null && x.Description.ToLower().Contains(lowerSearchTerm)));
            }

            // Filter by band from
            if (fromBandFrom.HasValue)
            {
                rewardSpendBands = rewardSpendBands.Where(x => x.BandFrom >= fromBandFrom);
            }

            if (toBandFrom.HasValue)
            {
                rewardSpendBands = rewardSpendBands.Where(x => x.BandFrom <= fromBandFrom);
            }

            // Filter by band from
            if (fromBandTo.HasValue)
            {
                rewardSpendBands = rewardSpendBands.Where(x => x.BandTo >= fromBandTo);
            }

            if (toBandTo.HasValue)
            {
                rewardSpendBands = rewardSpendBands.Where(x => x.BandTo <= toBandTo);
            }

            // Filter by percentage reward
            if (fromPercentageReward.HasValue)
            {
                rewardSpendBands = rewardSpendBands.Where(x => x.PercentageReward >= fromPercentageReward);
            }

            if (toPercentageReward.HasValue)
            {
                rewardSpendBands = rewardSpendBands.Where(x => x.PercentageReward <= toPercentageReward);
            }

            // Filter by percentage reward
            if (fromUpTo.HasValue)
            {
                rewardSpendBands = rewardSpendBands.Where(x => x.UpTo >= fromUpTo);
            }

            if (toUpTo.HasValue)
            {
                rewardSpendBands = rewardSpendBands.Where(x => x.UpTo <= toUpTo);
            }

            // Filter by band type
            if (bandType != null)
            {
                rewardSpendBands = rewardSpendBands.Where(x => x.Type <= bandType);
            }

            return rewardSpendBands;
        }

        /// <summary>
        /// Orders reward spend bands in a queryable list
        /// </summary>
        /// <param name="users">The list to order</param>
        /// <param name="sortOrder">The sort order details</param>
        /// <returns>Sorted reward spend band list</returns>
        private IQueryable<CryptoRewardSpendBand> OrderRewardSpendBands(IQueryable<CryptoRewardSpendBand> users, SortOrder sortOrder)
        {
            // Sort users where supported property exists - default name
            return (sortOrder.OrderProperty.Trim(), sortOrder.Order) switch
            {
                ("createdDate", Order.Ascending) => users.OrderBy(x => x.CreatedDate),
                ("createdDate", Order.Descending) => users.OrderByDescending(x => x.CreatedDate),
                ("name", Order.Ascending) => users.OrderBy(x => x.Name),
                ("name", Order.Descending) => users.OrderByDescending(x => x.Name),
                ("description", Order.Ascending) => users.OrderBy(x => x.Description),
                ("description", Order.Descending) => users.OrderByDescending(x => x.Description),
                ("bandFrom", Order.Ascending) => users.OrderBy(x => x.BandFrom),
                ("bandFrom", Order.Descending) => users.OrderByDescending(x => x.BandFrom),
                ("bandTo", Order.Ascending) => users.OrderBy(x => x.BandTo),
                ("bandTo", Order.Descending) => users.OrderByDescending(x => x.BandTo),
                ("percentageReward", Order.Ascending) => users.OrderBy(x => x.PercentageReward),
                ("percentageReward", Order.Descending) => users.OrderByDescending(x => x.PercentageReward),
                _ => sortOrder.Order == Order.Ascending ? users.OrderBy(x => x.CreatedDate) : users.OrderByDescending(x => x.CreatedDate)
            };
        }

        #endregion
    }
}
