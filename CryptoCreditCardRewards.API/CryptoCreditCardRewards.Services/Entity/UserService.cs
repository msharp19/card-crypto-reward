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
    public class UserService : IUserService
    {
        private readonly CryptoCreditCardRewardsDbContext _context;

        public UserService(CryptoCreditCardRewardsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="email">The users email (unique)</param>
        /// <param name="accountNumber">The users account number (unique)</param>
        /// <param name="active">The active state of the user (defines account accessability)</param>
        /// <returns>The new user</returns>
        public async Task<User> CreateUserAsync(string email, string accountNumber, bool completedKyc, bool active = true)
        {
            // Create user
            var user = new User(active, email, accountNumber);

            // Complete KYC if possible
            if (completedKyc)
                user.CompleteKyc();

            // Persist
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Return
            return user;
        }

        /// <summary>
        /// Validates if an account number provided is unique
        /// </summary>
        /// <param name="email">The account number to check</param>
        /// <returns>True if the account number provided is unique</returns>
        public bool IsAccountNumberUnique(string email)
        {
            return !_context.Users.Any(x => x.AccountNumber.ToLower() == email.ToLower());
        }

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
        public PagedResults<User> GetUsersPaged(string? search, string? email, string? accountNumber, bool? kycComplete, ActiveState state, Page page, SortOrder sortOrder)
        {
            // Get users query
            var users = GetUsersQuery(search, email, accountNumber, kycComplete, state);

            // Sort 
            users = OrderUsers(users, sortOrder);

            // Paginate
            var results = users.Skip((int)(page.PageIndex * page.PerPage))
                .Take((int)page.PerPage)
                .ToList();

            // Get total
            var total = GetUsersQuery(search, email, accountNumber, kycComplete, state).Select(x => x.Id).Count();

            // Map and return
            return new PagedResults<User>()
            {
                Items = results,
                Page = page,
                SortOrder = sortOrder,
                TotalCount = total
            };
        }

        /// <summary>
        /// Validates if an emailr provided is unique
        /// </summary>
        /// <param name="email">The email to check</param>
        /// <returns>True if the email provided is unique</returns>
        public bool IsEmailUnique(string email)
        {
            return !_context.Users.Any(x => x.Email.ToLower() == email.ToLower());
        }

        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="userId">The user to get</param>
        /// <param name="state">If the user is active or not</param>
        /// <returns>A user if exists</returns>
        public User? GetUser(int userId, ActiveState state = ActiveState.Active)
        {
            // Get users
            var users = _context.Users.AsQueryable();

            // Filter by active state
            if (state == ActiveState.Active)
                users = users.Where(x => x.Active);
            else if (state == ActiveState.InActive)
                users = users.Where(x => !x.Active);

            // Filter by id
            return users.FirstOrDefault(x => x.Id == userId);
        }

        /// <summary>
        /// Deactivate a user
        /// </summary>
        /// <param name="id">The user to deactivate</param>
        /// <returns>An async task</returns>
        public async Task DeactivateUserAsync(int id)
        {
            // Get user
            var user = GetUser(id);

            // Update
            user.Deactivate();
            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Reactivate a user
        /// </summary>
        /// <param name="id">The user to reactivate</param>
        /// <returns>The active user</returns>
        public async Task<User> ReactivateUserAsync(int id)
        {
            // Get user
            var user = GetUser(id, ActiveState.Both);

            // Update
            user.Activate();
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Complete an accounts KYC
        /// </summary>
        /// <param name="id">The user to update</param>
        /// <returns>The updated user</returns>
        public async Task<User> CompleteKycAsync(int id)
        {
            // Get user
            var user = GetUser(id);

            // Update
            user.CompleteKyc();
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return user;
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
                new ("completedKycDate", Order.Ascending),
                new ("completedKycDate", Order.Descending),
            };
        }

        #region Helpers

        private IQueryable<User> GetUsersQuery(string? search, string? email, string? accountNumber, bool? kycComplete, ActiveState state)
        {
            var users = _context.Users.AsQueryable();

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearchTerm = search.ToLower().Trim();
                users = users.Where(x => x.Email.ToLower().Contains(lowerSearchTerm) ||
                     x.AccountNumber.ToLower().Contains(lowerSearchTerm));
            }

            // Filter by email
            if (!string.IsNullOrEmpty(email))
            {
                users = users.Where(x => x.Email.ToLower().Trim() == email.ToLower().Trim());
            }

            // Filter by account number
            if (!string.IsNullOrEmpty(accountNumber))
            {
                users = users.Where(x => x.AccountNumber.ToLower().Trim() == accountNumber.ToLower().Trim());
            }

            // Filter by completed kyc
            if(kycComplete.HasValue)
            {
                users = users.Where(x => x.CompletedKycDate != null);
            }

            // Filter by state
            if (state == ActiveState.Active)
            {
                users = users.Where(x => x.Active);
            }
            else if (state == ActiveState.InActive)
            {
                users = users.Where(x => !x.Active);
            }

            return users;
        }

        /// <summary>
        /// Orders users in a queryable list
        /// </summary>
        /// <param name="users">The list to order</param>
        /// <param name="sortOrder">The sort order details</param>
        /// <returns>Sorted user list</returns>
        private IQueryable<User> OrderUsers(IQueryable<User> users, SortOrder sortOrder)
        {
            // Sort users where supported property exists - default name
            return (sortOrder.OrderProperty.Trim(), sortOrder.Order) switch
            {
                ("createdDate", Order.Ascending) => users.OrderBy(x => x.CreatedDate),
                ("createdDate", Order.Descending) => users.OrderByDescending(x => x.CreatedDate),
                ("completedKycDate", Order.Ascending) => users.OrderBy(x => x.CompletedKycDate),
                ("completedKycDate", Order.Descending) => users.OrderByDescending(x => x.CompletedKycDate),
                _ => sortOrder.Order == Order.Ascending ? users.OrderBy(x => x.CreatedDate) : users.OrderByDescending(x => x.CreatedDate)
            };
        }

        #endregion
    }
}
