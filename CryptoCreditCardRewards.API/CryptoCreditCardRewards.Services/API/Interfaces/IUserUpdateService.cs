using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface IUserUpdateService
    {
        /// <summary>
        /// Deactivate a user
        /// </summary>
        /// <param name="id">The user to deactivate</param>
        /// <returns>An async task</returns>
        Task DeactivateUserAsync(int id);

        /// <summary>
        /// Reactivate a user
        /// </summary>
        /// <param name="id">The user to reactivate</param>
        /// <returns>The active user</returns>
        Task<User> ReactivateUserAsync(int id);

        /// <summary>
        /// Complete an accounts KYC
        /// </summary>
        /// <param name="id">The user to update</param>
        /// <returns>The updated user</returns>
        Task<User> CompleteKycAsync(int id);
    }
}
