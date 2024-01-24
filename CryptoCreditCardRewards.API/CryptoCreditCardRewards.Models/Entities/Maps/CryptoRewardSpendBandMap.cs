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
    public class CryptoRewardSpendBandMap
    {
        public CryptoRewardSpendBandMap(EntityTypeBuilder<CryptoRewardSpendBand> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("CryptoRewardBands", "dbo");
            entityTypeBuilder.HasKey(t => t.Id);
            entityTypeBuilder.Property(t => t.Id).UseIdentityColumn();

            entityTypeBuilder.Property(e => e.Type).HasConversion(new EnumToStringConverter<BandType>());
        }
    }
}
