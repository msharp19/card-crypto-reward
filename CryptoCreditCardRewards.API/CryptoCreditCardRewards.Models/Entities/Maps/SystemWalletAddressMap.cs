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
    public class SystemWalletAddressMap
    {
        public SystemWalletAddressMap(EntityTypeBuilder<SystemWalletAddress> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("SystemWalletAddresses", "dbo");
            entityTypeBuilder.HasKey(t => t.Id);
            entityTypeBuilder.Property(t => t.Id).UseIdentityColumn();

            entityTypeBuilder.Property(e => e.AddressType).HasConversion(new EnumToStringConverter<AddressType>());

            entityTypeBuilder.HasOne(x => x.CryptoCurrency).WithMany(x => x.SystemWalletAddresses);
            entityTypeBuilder.HasMany(x => x.Transactions).WithOne(x => x.SystemWalletAddress);
        }
    }
}
