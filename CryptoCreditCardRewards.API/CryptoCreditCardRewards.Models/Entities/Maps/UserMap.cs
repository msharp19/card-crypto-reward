using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Entities.Maps
{
    public class UserMap
    {
        public UserMap(EntityTypeBuilder<User> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("Users", "dbo");
            entityTypeBuilder.HasKey(t => t.Id);
            entityTypeBuilder.Property(t => t.Id).UseIdentityColumn();

            entityTypeBuilder.HasMany(x => x.Transactions).WithOne(x => x.User);
            entityTypeBuilder.HasMany(x => x.WalletAddresses).WithOne(x => x.User);
            entityTypeBuilder.HasMany(x => x.UserRewardSelections).WithOne(x => x.User);
            entityTypeBuilder.HasMany(x => x.Instructions).WithOne(x => x.User);
            entityTypeBuilder.HasMany(x => x.WhitelistAddresses).WithOne(x => x.User);
        }
    }
}
