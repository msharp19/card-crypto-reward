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
    public class CryptoCurrencyMap
    {
        public CryptoCurrencyMap(EntityTypeBuilder<CryptoCurrency> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("CryptoCurrencies", "dbo");
            entityTypeBuilder.HasKey(t => t.Id);
            entityTypeBuilder.Property(t => t.Id).UseIdentityColumn();

            entityTypeBuilder.Property(e => e.InfrastructureType).HasConversion(new EnumToStringConverter<InfrastructureType>());
            entityTypeBuilder.Property(e => e.ConversionServiceType).HasConversion(new EnumToStringConverter<ConversionServiceType>());

            entityTypeBuilder.HasMany(x => x.WalletAddresses).WithOne(x => x.CryptoCurrency);
            entityTypeBuilder.HasMany(x => x.SystemWalletAddresses).WithOne(x => x.CryptoCurrency);
            entityTypeBuilder.HasMany(x => x.Transactions).WithOne(x => x.CryptoCurrency);
            entityTypeBuilder.HasMany(x => x.UserRewardSelections).WithOne(x => x.CryptoCurrency);
            entityTypeBuilder.HasMany(x => x.WhitelistAddresses).WithOne(x => x.CryptoCurrency);
        }
    }
}
