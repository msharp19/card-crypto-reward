using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Constants;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Entities.Audit;
using CryptoCreditCardRewards.Models.Entities.Maps;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Utilities;

namespace CryptoCreditCardRewards.Contexts
{
    /// <summary>
    /// Database context
    /// </summary>
    public class CryptoCreditCardRewardsDbContext : DbContext
    {
        private readonly IMediator _mediator;

        protected int? UserId => GetLoggedInUserId();

        public DbSet<User> Users { get; set; }
        public DbSet<CryptoCurrency> CryptoCurrencies { get; set; }
        public DbSet<CryptoRewardSpendBand> CryptoRewardSpendBands { get; set; }
        public DbSet<WalletAddress> WalletAddresses { get; set; }
        public DbSet<SystemWalletAddress> SystemWalletAddresses { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<UserAction> UserActions { get; set; }
        public DbSet<UserRewardSelection> UserRewardSelections { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<WhitelistAddress> WhitelistAddresses { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public CryptoCreditCardRewardsDbContext(DbContextOptions<CryptoCreditCardRewardsDbContext> options)
            : base(options)
        {
            _mediator = this.GetService<IMediator>();

            Database.EnsureCreated();
        }

        /// <summary>
        /// Called on model creation (mapping can be done here)
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Add JSON_VALUE column
            builder.HasDbFunction(() => JsonExtensions.JsonValue(default(string), default(string)));

            // Map entities
            _ = new UserMap(builder.Entity<User>());
            _ = new TransactionMap(builder.Entity<Transaction>());
            _ = new WalletAddressMap(builder.Entity<WalletAddress>());
            _ = new SystemWalletAddressMap(builder.Entity<SystemWalletAddress>());
            _ = new CryptoRewardSpendBandMap(builder.Entity<CryptoRewardSpendBand>());
            _ = new CryptoCurrencyMap(builder.Entity<CryptoCurrency>());
            _ = new UserActionMap(builder.Entity<UserAction>());
            _ = new UserRewardSelectionMap(builder.Entity<UserRewardSelection>());
            _ = new InstructionMap(builder.Entity<Instruction>());
            _ = new WhitelistAddressMap(builder.Entity<WhitelistAddress>());

            base.OnModelCreating(builder);
        }

        /// <summary>
        /// Save changes to the context
        /// </summary>
        /// <returns>Number of rows updated</returns>
        public override int SaveChanges()
        {
            // Populate the audit information
            AddAuditProperties();

            // Save
            var response = base.SaveChanges();

            // Dispatch events
            DispatchDomainEvents().GetAwaiter().GetResult();

            // Return update response
            return response;
        }

        /// <summary>
        /// Save changes to the context asynchronously
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for operation shutdown</param>
        /// <returns>Number of roes updated</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Populate the audit information
            AddAuditProperties();

            // Save
            var response = await base.SaveChangesAsync(cancellationToken);

            // Dispatch events
            await DispatchDomainEvents();

            // Return update response
            return response;
        }

        /// <summary>
        /// Clear the change tracker
        /// </summary>
        public void ClearChangeTracker()
        {
            ChangeTracker.Clear();
        }

        #region Helpers

        /// <summary>
        /// Add audit to entity being persisted (if accepts audit)
        /// </summary>
        private void AddAuditProperties()
        {
            foreach (EntityEntry<IAuditableProperties> entry in ChangeTracker.Entries<IAuditableProperties>())
            {
                // Update the inserted audit values
                if (entry.State == EntityState.Added)
                {
                    entry.Property(AuditProperty.CreatedById.ToString()).CurrentValue = string.IsNullOrEmpty((string)entry.Property(AuditProperty.CreatedById.ToString()).CurrentValue)
                        ? UserId : entry.Property(AuditProperty.CreatedById.ToString()).CurrentValue;
                    entry.Property(AuditProperty.CreatedDate.ToString()).CurrentValue = (entry.Property(AuditProperty.CreatedDate.ToString()).CurrentValue != null) ? DateTime.UtcNow :
                        entry.Property(AuditProperty.CreatedDate.ToString()).CurrentValue;
                }
                // Update the updated audit values
                else if (entry.State == EntityState.Modified) // Existing entity updated
                {
                    entry.Property(AuditProperty.UpdatedById.ToString()).CurrentValue = UserId;
                    entry.Property(AuditProperty.UpdatedDate.ToString()).CurrentValue = DateTime.UtcNow;
                }
            }
        }

        private int? GetLoggedInUserId()
        {
            // Get username from claims
            var rawUserId = this.GetService<IHttpContextAccessor>().HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == CryptoCreditCardRewardsClaimNames.UserId)?.Value;

            // Parse it and return if resolved
            var parsed = int.TryParse(rawUserId, out int userId);
            if (parsed)
                return userId;

            // Otherwise return null
            return null;
        }

        private async Task DispatchDomainEvents()
        {
            var domainEventEntities = ChangeTracker.Entries<BaseEntity>()
                .Select(po => po.Entity)
                .Where(po => po.DomainEvents.Any())
                .ToArray();

            foreach (var entity in domainEventEntities)
            {
                var events = entity.DomainEvents.ToArray();
                entity.DomainEvents.Clear();
                foreach (var entityDomainEvent in events)
                    await _mediator.Publish(entityDomainEvent);
            }
        }

        #endregion
    }
}
