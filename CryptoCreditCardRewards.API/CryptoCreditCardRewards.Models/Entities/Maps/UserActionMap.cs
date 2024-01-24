using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Entities.Maps
{
    public class UserActionMap
    {
        public UserActionMap(EntityTypeBuilder<UserAction> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("UserActions", "dbo");
            entityTypeBuilder.HasKey(t => t.Id);
            entityTypeBuilder.Property(t => t.Id).UseIdentityColumn();

            entityTypeBuilder.Property(e => e.ActionType).HasConversion(new EnumToStringConverter<UserActionType>());

            entityTypeBuilder.HasOne(x => x.Transaction).WithMany(x => x.UserActions);
            entityTypeBuilder.HasOne(x => x.WalletAddress).WithMany(x => x.UserActions);
        }
    }
}
