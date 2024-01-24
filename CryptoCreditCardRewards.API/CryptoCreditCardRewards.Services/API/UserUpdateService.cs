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
    public class UserUpdateService : IUserUpdateService
    {
        private readonly IUserService _userService;

        public UserUpdateService(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Deactivate a user
        /// </summary>
        /// <param name="id">The user to deactivate</param>
        /// <returns>An async task</returns>
        public async Task DeactivateUserAsync(int id)
        {
            // Get user to deactivate
            var existingUser = _userService.GetUser(id);
            if (existingUser == null)
                throw new NotFoundException(FailedReason.UserDoesntExist, Property.Id);

            // Check not already deactivated
            if (!existingUser.Active)
                throw new UnprocessableEntityException(FailedReason.UserAlreadyDeactivated, Property.Id);

            // Deactivate
            await _userService.DeactivateUserAsync(id);

            return;
        }

        /// <summary>
        /// Reactivate a user
        /// </summary>
        /// <param name="id">The user to reactivate</param>
        /// <returns>The active user</returns>
        public async Task<User> ReactivateUserAsync(int id)
        {
            // Get user to deactivate
            var existingUser = _userService.GetUser(id, ActiveState.Both);
            if (existingUser == null)
                throw new NotFoundException(FailedReason.UserDoesntExist, Property.Id);

            // Check not already deactivated
            if (existingUser.Active)
                throw new UnprocessableEntityException(FailedReason.UserAlreadyActive, Property.Id);

            // Deactivate
            return await _userService.ReactivateUserAsync(id);
        }

        /// <summary>
        /// Complete an accounts KYC
        /// </summary>
        /// <param name="id">The user to update</param>
        /// <returns>The updated user</returns>
        public async Task<User> CompleteKycAsync(int id)
        {
            // Get user to complete kyc for
            var existingUser = _userService.GetUser(id);
            if (existingUser == null)
                throw new NotFoundException(FailedReason.UserDoesntExist, Property.Id);

            // Check not already kyc complete
            if (existingUser.CompletedKycDate.HasValue)
                throw new UnprocessableEntityException(FailedReason.UserAlreadyCompletedKyc, Property.Id);

            // Complete
            return await _userService.CompleteKycAsync(id);
        }
    }
}
