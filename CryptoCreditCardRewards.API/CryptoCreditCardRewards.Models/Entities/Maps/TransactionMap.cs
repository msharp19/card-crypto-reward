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
    public class TransactionMap
    {
        public TransactionMap(EntityTypeBuilder<Transaction> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("Transactions", "dbo");
            entityTypeBuilder.HasKey(t => t.Id);
            entityTypeBuilder.Property(t => t.Id).UseIdentityColumn();

            entityTypeBuilder.Property(e => e.Type).HasConversion(new EnumToStringConverter<TransactionType>());
            entityTypeBuilder.Property(e => e.State).HasConversion(new EnumToStringConverter<TransactionState>());

            entityTypeBuilder.HasOne(x => x.CryptoCurrency).WithMany(x => x.Transactions);
            entityTypeBuilder.HasOne(x => x.User).WithMany(x => x.Transactions);
            entityTypeBuilder.HasOne(x => x.WhitelistAddress).WithMany(x => x.Transactions);
            entityTypeBuilder.HasOne(x => x.Instruction).WithMany(x => x.Transactions);
            entityTypeBuilder.HasOne(x => x.WalletAddress).WithMany(x => x.Transactions);
            entityTypeBuilder.HasOne(x => x.SystemWalletAddress).WithMany(x => x.Transactions);
            entityTypeBuilder.HasMany(x => x.UserActions).WithOne(x => x.Transaction);
        }
    }
}
