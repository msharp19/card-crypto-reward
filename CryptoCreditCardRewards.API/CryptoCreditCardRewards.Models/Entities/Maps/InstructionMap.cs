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
    public class InstructionMap
    {
        public InstructionMap(EntityTypeBuilder<Instruction> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("Instructions", "dbo");
            entityTypeBuilder.HasKey(t => t.Id);
            entityTypeBuilder.Property(t => t.Id).UseIdentityColumn();

            entityTypeBuilder.Property(e => e.Type).HasConversion(new EnumToStringConverter<InstructionType>());

            entityTypeBuilder.HasOne(x => x.User).WithMany(x => x.Instructions);
            entityTypeBuilder.HasOne(x => x.WalletAddress).WithMany(x => x.Instructions);
            entityTypeBuilder.HasOne(x => x.WhitelistAddress).WithMany(x => x.Instructions);
            entityTypeBuilder.HasMany(x => x.Transactions).WithOne(x => x.Instruction);
        }
    }
}
