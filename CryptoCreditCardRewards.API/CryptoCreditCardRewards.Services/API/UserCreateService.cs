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
    public class UserCreateService : IUserCreateService
    {
        private readonly IUserService _userService;

        public UserCreateService(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="email">The users email (unique)</param>
        /// <param name="accountNumber">The users account number (unique)</param>
        /// <param name="completedKyc">If the user has completed KYC</param>
        /// <param name="active">The active state of the user (defines account accessability)</param>
        /// <returns>The new user</returns>
        public async Task<User> CreateUserAsync(string email, string accountNumber, bool completedKyc, bool active)
        {
            // Validate account number
            if (!_userService.IsAccountNumberUnique(accountNumber))
                throw new BadRequestException(FailedReason.AccountNumberIsNotUnique, Property.AccountNumber);

            // Validate email
            if (!_userService.IsEmailUnique(email))
                throw new BadRequestException(FailedReason.EmailIsNotUnique, Property.Email);

            // Create the new user
            var newUser = await _userService.CreateUserAsync(email, accountNumber, completedKyc, active);

            // Return
            return newUser;
        }
    }
}
