using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface IUserCreateService
    {
        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="email">The users email (unique)</param>
        /// <param name="accountNumber">The users account number (unique)</param>
        /// <param name="completedKyc">If the user has completed KYC</param>
        /// <param name="active">The active state of the user (defines account accessability)</param>
        /// <returns>The new user</returns>
        public Task<User> CreateUserAsync(string email, string accountNumber, bool completedKyc, bool active);
    }
}
