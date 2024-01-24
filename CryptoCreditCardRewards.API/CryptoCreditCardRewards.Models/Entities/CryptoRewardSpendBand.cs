using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities.Audit;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Entities
{
    public class CryptoRewardSpendBand : BaseEntity
    {
        public int Id { get; private set; }
        public bool Active { get; private set; }
        public string? Name { get; private set; }
        public string? Description { get; private set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal BandFrom { get; private set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal BandTo { get; private set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal UpTo { get; private set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal PercentageReward { get; private set; }
        public BandType Type { get; private set; }


        public CryptoRewardSpendBand(bool active, string? name, string? description, decimal bandFrom, decimal bandTo, decimal upTo, decimal percentageReward, BandType type)
        {
            Active = active;
            Name = name;
            UpTo = upTo;
            Description = description;
            BandFrom = bandFrom;
            BandTo = bandTo;
            PercentageReward = percentageReward;
            Type = type;
        }
    }
}
