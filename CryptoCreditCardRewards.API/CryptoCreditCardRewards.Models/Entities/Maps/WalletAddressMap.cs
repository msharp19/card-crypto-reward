using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Entities.Maps
{
    public class WalletAddressMap
    {
        public WalletAddressMap(EntityTypeBuilder<WalletAddress> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("WalletAddresses", "dbo");
            entityTypeBuilder.HasKey(t => t.Id);
            entityTypeBuilder.Property(t => t.Id).UseIdentityColumn();

            entityTypeBuilder.HasOne(x => x.CryptoCurrency).WithMany(x => x.WalletAddresses);
            entityTypeBuilder.HasOne(x => x.User).WithMany(x => x.WalletAddresses);
            entityTypeBuilder.HasMany(x => x.UserActions).WithOne(x => x.WalletAddress);
            entityTypeBuilder.HasMany(x => x.Instructions).WithOne(x => x.WalletAddress);
            entityTypeBuilder.HasMany(x => x.Transactions).WithOne(x => x.WalletAddress);
        }
    }
}
