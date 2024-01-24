using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Contexts;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.Services.Entity
{
    public class UserRewardSelectionService : IUserRewardSelectionService
    {
        private readonly CryptoCreditCardRewardsDbContext _context;

        public UserRewardSelectionService(CryptoCreditCardRewardsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create a new user reward selection
        /// </summary>
        /// <param name="userId">The user id to create the selection for</param>
        /// <param name="cryptoCurrencyId">The crypto currency to create the reward selection for</param>
        /// <returns>The new reward selection</returns>
        public async Task<UserRewardSelection> CreateUserRewardSelectionAsync(int userId, int cryptoCurrencyId)
        {
            // Get the amount we can set as contribution %
            var contributionPercentage = GetRemainingContributionPercentage(userId);

            // Create
            var newUserRewardSelection = new UserRewardSelection(cryptoCurrencyId, userId, contributionPercentage);

            // Save
            _context.UserRewardSelections.Add(newUserRewardSelection);
            await _context.SaveChangesAsync();

            return newUserRewardSelection;
        }

        /// <summary>
        /// Get a set of reward selections by their ids
        /// </summary>
        /// <param name="ids">The ids to get the reward selections by</param>
        /// <returns>Reward selections</returns>
        public List<UserRewardSelection> GetAllRewardSelectionsByIds(params int[] ids)
        {
            return _context.UserRewardSelections.Where(x => ids.Contains(x.Id))
                .ToList();
        }

        /// <summary>
        /// Update a set of reward selections
        /// </summary>
        /// <param name="selectionsToUpdate">The reward selections to update</param>
        /// <returns>The updated reward selections</returns>
        public async Task<List<UserRewardSelection>> UpdateUserRewardSelectionAsync(List<UserRewardSelection> selectionsToUpdate)
        {
            // Just get the selections to updates ids
            var selectionToUpdateIds = selectionsToUpdate.Select(x => x.Id).ToArray();

            // Get all back to update
            var rewardSelections = GetAllRewardSelectionsByIds(selectionToUpdateIds);

            foreach (var selectionToUpdate in selectionsToUpdate)
            {
                var matching = rewardSelections.FirstOrDefault(x => x.Id == selectionToUpdate.Id);
                if(matching != null)
                {
                    matching.SetContributionPercentage(selectionToUpdate.ContributionPercentage);
                }
            }

            // Update
            _context.UserRewardSelections.UpdateRange(rewardSelections);
            await _context.SaveChangesAsync();

            return rewardSelections;
        }

        /// <summary>
        /// Gets the remaining amount of contribution percentage which can be added to a reward selection
        /// </summary>
        /// <param name="userId">The user id to check against</param>
        /// <param name="cryptoCurrencyId">The crypto currency id to check against</param>
        /// <returns>The contribution percentage left to add (100-used)</returns>
        public decimal GetRemainingContributionPercentage(int userId)
        {
            return 100 - _context.UserRewardSelections.Where(x => x.UserId == userId)
                .Sum(x => x.ContributionPercentage);
        }

        /// <summary>
        /// Get a page of a users reward selections
        /// </summary>
        /// <param name="userId">The user to get reward selections for</param>
        /// <param name="search">A search term (crypto currency name)</param>
        /// <param name="cryptoCurrencyId">A crypto currency id</param>
        /// <param name="page">The page to get</param>
        /// <param name="sortOrder">The order to return the page</param>
        /// <returns>Paged user reward selections</returns>
        public PagedResults<UserRewardSelection> GetUserRewardSelectionsPaged(int userId, string? search, int? cryptoCurrencyId, Page page, SortOrder sortOrder)
        {
            // Get users query
            var users = GetUserRewardSelectionsQuery(userId, search, cryptoCurrencyId);

            // Sort 
            users = OrderUserRewardSelections(users, sortOrder);

            // Paginate
            var results = users.Skip((int)(page.PageIndex * page.PerPage))
                .Take((int)page.PerPage)
                .ToList();

            // Get total
            var total = GetUserRewardSelectionsQuery(userId, search, cryptoCurrencyId).Select(x => x.Id).Count();

            // Map and return
            return new PagedResults<UserRewardSelection>()
            {
                Items = results,
                Page = page,
                SortOrder = sortOrder,
                TotalCount = total
            };
        }

        /// <summary>
        /// Get a reward selection
        /// </summary>
        /// <param name="id">The reward selection</param>
        /// <param name="state">The active state of the reward selection</param>
        /// <returns>A reward selection by id</returns>
        public UserRewardSelection? GetRewardSelection(int id)
        {
            return _context.UserRewardSelections.Include(x => x.CryptoCurrency)
               .Include(x => x.User)
               .FirstOrDefault();
        }

        /// <summary>
        /// Get all reward selections for a user
        /// </summary>
        /// <param name="userId">The user to get reward selections for</param>
        /// <returns>A users reward selections</returns>
        public List<UserRewardSelection> GetUserRewardSelections(int userId)
        {
            return _context.UserRewardSelections.Where(x => x.UserId == userId)
                .ToList();
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
                new ("contributionPercentage", Order.Ascending),
                new ("contributionPercentage", Order.Descending),
            };
        }

        #region Helpers

        /// <summary>
        /// Orders users reward selections in a queryable list
        /// </summary>
        /// <param name="userRewardSelections">The list to order</param>
        /// <param name="sortOrder">The sort order details</param>
        /// <returns>Sorted user list</returns>
        private IQueryable<UserRewardSelection> OrderUserRewardSelections(IQueryable<UserRewardSelection> userRewardSelections, SortOrder sortOrder)
        {
            // Sort users where supported property exists - default name
            return (sortOrder.OrderProperty.Trim(), sortOrder.Order) switch
            {
                ("createdDate", Order.Ascending) => userRewardSelections.OrderBy(x => x.CreatedDate),
                ("createdDate", Order.Descending) => userRewardSelections.OrderByDescending(x => x.CreatedDate),
                ("contributionPercentage", Order.Ascending) => userRewardSelections.OrderBy(x => x.ContributionPercentage),
                ("contributionPercentage", Order.Descending) => userRewardSelections.OrderByDescending(x => x.ContributionPercentage),
                _ => sortOrder.Order == Order.Ascending ? userRewardSelections.OrderBy(x => x.CreatedDate) : userRewardSelections.OrderByDescending(x => x.CreatedDate)
            };
        }

        private IQueryable<UserRewardSelection> GetUserRewardSelectionsQuery(int userId, string? search, int? cryptoCurrencyId)
        {
            // Get reward selections
            var userRewardSelections = _context.UserRewardSelections
                .Include(x => x.User)
                .Include(x => x.CryptoCurrency)
                .Where(x => x.UserId == userId);

            // Filter by search term
            if(!string.IsNullOrEmpty(search))
            {
                // Format search term
                search = search.Trim().ToLower();

                // Filter by search term
                userRewardSelections = userRewardSelections.Where(x => x.CryptoCurrency != null)
                    .Where(x => x.CryptoCurrency.Name != null)
                    .Where(x => x.CryptoCurrency.Name.ToLower().Contains(search));
            }

            // Filter by crypto currency
            if(cryptoCurrencyId.HasValue)
            {
                userRewardSelections = userRewardSelections.Where(x => x.CryptoCurrencyId == cryptoCurrencyId);
            }

            return userRewardSelections;
        }

        #endregion
    }
}
