using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Entities.Maps
{
    public class UserRewardSelectionMap
    {
        public UserRewardSelectionMap(EntityTypeBuilder<UserRewardSelection> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("UserRewardSelections", "dbo");
            entityTypeBuilder.HasKey(t => t.Id);
            entityTypeBuilder.Property(t => t.Id).UseIdentityColumn();

            entityTypeBuilder.HasOne(x => x.User).WithMany(x => x.UserRewardSelections);
            entityTypeBuilder.HasOne(x => x.CryptoCurrency).WithMany(x => x.UserRewardSelections);
        }
    }
}
