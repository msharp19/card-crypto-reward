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
    public interface IUserService
    {
        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="email">The users email (unique)</param>
        /// <param name="accountNumber">The users account number (unique)</param>
        /// <param name="active">The active state of the user (defines account accessability)</param>
        /// <returns>The new user</returns>
        Task<User> CreateUserAsync(string email, string accountNumber, bool completedKyc, bool active = true);

        /// <summary>
        /// Validates if an account number provided is unique
        /// </summary>
        /// <param name="accountNumber">The account number to check</param>
        /// <returns>True if the account number provided is unique</returns>
        bool IsAccountNumberUnique(string accountNumber);
        
        /// <summary>
        /// Get a list of paged users
        /// </summary>
        /// <param name="search"></param>
        /// <param name="email"></param>
        /// <param name="accountNumber"></param>
        /// <param name="state"></param>
        /// <param name="page"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        PagedResults<User> GetUsersPaged(string? search, string? email, string? accountNumber, bool? kycComplete, ActiveState state, Page page, SortOrder sortOrder);

        /// <summary>
        /// Validates if an emailr provided is unique
        /// </summary>
        /// <param name="email">The email to check</param>
        /// <returns>True if the email provided is unique</returns>
        bool IsEmailUnique(string email);


        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="userId">The user to get</param>
        /// <param name="state">If the user is active or not</param>
        /// <returns>A user if exists</returns>
        User? GetUser(int userId, ActiveState state = ActiveState.Active);

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
        /// Gets the properties to sort by
        /// </summary>
        /// <returns>All properties to sort by</returns>
        List<(string PropertyName, Order Order)> GetSortProperties();

        /// <summary>
        /// Complete an accounts KYC
        /// </summary>
        /// <param name="id">The user to update</param>
        /// <returns>The updated user</returns>
        Task<User> CompleteKycAsync(int id);
    }
}
