using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Entities.Maps
{
    public class WhitelistAddressMap
    {
        public WhitelistAddressMap(EntityTypeBuilder<WhitelistAddress> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("WhitelistAddresses", "dbo");
            entityTypeBuilder.HasKey(t => t.Id);
            entityTypeBuilder.Property(t => t.Id).UseIdentityColumn();

            entityTypeBuilder.HasOne(x => x.User).WithMany(x => x.WhitelistAddresses);
            entityTypeBuilder.HasOne(x => x.CryptoCurrency).WithMany(x => x.WhitelistAddresses);
            entityTypeBuilder.HasMany(x => x.Instructions).WithOne(x => x.WhitelistAddress);
            entityTypeBuilder.HasMany(x => x.Transactions).WithOne(x => x.WhitelistAddress);
        }
    }
}
