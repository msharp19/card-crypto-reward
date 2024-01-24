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
    public class UserRewardSelectionUpdateService : IUserRewardSelectionUpdateService
    {
        private readonly IUserRewardSelectionService _userRewardSelectionService;
        private readonly IUserService _userService;

        public UserRewardSelectionUpdateService(IUserRewardSelectionService userRewardSelectionService, IUserService userService)
        {
            _userRewardSelectionService = userRewardSelectionService;
            _userService = userService;
        }

        /// <summary>
        /// Create a users reward selection 
        /// </summary>
        /// <param name="userId">The user to update selction % for</param>
        /// <param name="selectionsToUpdate">The selections to create</param>
        /// <returns>The new reward selection</returns>
        public async Task<List<UserRewardSelection>> UpdateUserRewardSelectionsAsync(int userId, List<UserRewardSelection> selectionsToUpdate)
        {
            // Check user exists
            var user = _userService.GetUser(userId);
            if (user == null)
                throw new NotFoundException(FailedReason.UserDoesntExist, Property.UserId);

            // Check all reward selections exist & link to user
            var existingSelections = _userRewardSelectionService.GetAllRewardSelectionsByIds(selectionsToUpdate.Select(x => x.Id).ToArray());
            selectionsToUpdate.ForEach(x =>
            {
                var matchingSelection = existingSelections.FirstOrDefault(y => y.Id == x.Id);
                if (matchingSelection == null)
                    throw new UnprocessableEntityException(FailedReason.UserRewardSelectionDoesntExist, Property.Id);

                if (matchingSelection.UserId != userId)
                    throw new UnprocessableEntityException(FailedReason.UserDoesntOwnRewardSelection, Property.Id);
            });

            // Ensure all selections add up too 100
            var totalContributionPercentage = selectionsToUpdate.Sum(x => x.ContributionPercentage);
            if (totalContributionPercentage > 100)
                throw new UnprocessableEntityException(FailedReason.TotalContributionIsGreaterThan100, Property.ContributionPercentage);
            if (totalContributionPercentage < 100)
                throw new UnprocessableEntityException(FailedReason.TotalContributionIsLessThan100, Property.ContributionPercentage);

            // Update & return
            return await _userRewardSelectionService.UpdateUserRewardSelectionAsync(selectionsToUpdate);
        }
    }
}
