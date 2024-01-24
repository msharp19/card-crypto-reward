using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Dtos.UserRewardSelections;
using CryptoCreditCardRewards.Models.Entities;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface IUserRewardSelectionUpdateService
    {
        /// <summary>
        /// Update a users reward selection/s
        /// </summary>
        /// <param name="userId">The user to update selction % for</param>
        /// <param name="selectionsToUpdate">The selections to update</param>
        /// <returns>The updated reward selections</returns>
        Task<List<UserRewardSelection>> UpdateUserRewardSelectionsAsync(int userId, List<UserRewardSelection> selectionsToUpdate);
    }
}
