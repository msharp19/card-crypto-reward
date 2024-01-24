using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Services.Entity.Interfaces
{
    public interface IUserRewardSelectionService
    {
        /// <summary>
        /// Create a new user reward selection
        /// </summary>
        /// <param name="userId">The user id to create the selection for</param>
        /// <param name="cryptoCurrencyId">The crypto currency to create the reward selection for</param>
        /// <returns>The new reward selection</returns>
        Task<UserRewardSelection> CreateUserRewardSelectionAsync(int userId, int cryptoCurrencyId);

        /// <summary>
        /// Get a set of reward selections by their ids
        /// </summary>
        /// <param name="ids">The ids to get the reward selections by</param>
        /// <returns>Reward selections</returns>
        List<UserRewardSelection> GetAllRewardSelectionsByIds(params int[] ids);

        /// <summary>
        /// Update a set of reward selections
        /// </summary>
        /// <param name="selectionsToUpdate">The reward selections to update</param>
        /// <returns>The updated reward selections</returns>
        Task<List<UserRewardSelection>> UpdateUserRewardSelectionAsync(List<UserRewardSelection> selectionsToUpdate);

        /// <summary>
        /// Gets the properties to sort by
        /// </summary>
        /// <returns>All properties to sort by</returns>
        List<(string PropertyName, Order Order)> GetSortProperties();

        /// <summary>
        /// Get a page of a users reward selections
        /// </summary>
        /// <param name="userId">The user to get reward selections for</param>
        /// <param name="search">A search term (crypto currency name)</param>
        /// <param name="cryptoCurrencyId">A crypto currency id</param>
        /// <param name="page">The page to get</param>
        /// <param name="sortOrder">The order to return the page</param>
        /// <returns>Paged user reward selections</returns>
        PagedResults<UserRewardSelection> GetUserRewardSelectionsPaged(int userId, string? search, int? cryptoCurrencyId, Page page, SortOrder sortOrder);
        
        /// <summary>
        /// Get all reward selections for a user
        /// </summary>
        /// <param name="userId">The user to get reward selections for</param>
        /// <returns>A users reward selections</returns>
        List<UserRewardSelection> GetUserRewardSelections(int userId);

        /// <summary>
        /// Get a reward selection
        /// </summary>
        /// <param name="id">The reward selection</param>
        /// <returns>A reward selection by id</returns>
        UserRewardSelection? GetRewardSelection(int id);

        /// <summary>
        /// Gets the remaining amount of contribution percentage which can be added to a reward selection
        /// </summary>
        /// <param name="userId">The user id to check against</param>
        /// <param name="cryptoCurrencyId">The crypto currency id to check against</param>
        /// <returns>The contribution percentage left to add (100-used)</returns>
        decimal GetRemainingContributionPercentage(int userId);
    }
}
