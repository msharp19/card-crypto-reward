using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Dtos.CryptoRewardSpendBands
{
    public class CreateCryptoRewardSpendBandDto
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("bandFrom")]
        public decimal BandFrom { get; set; }

        [JsonProperty("bandTo")]
        public decimal BandTo { get; set; }

        [JsonProperty("upTo")]
        public decimal UpTo { get; set; }

        [JsonProperty("percentageReward")]
        public decimal PercentageReward { get; set; }

        [JsonProperty("bandType")]
        public BandType BandType { get; set; }

        [JsonProperty("cryptoCurrencyId")]
        public int? CryptoCurrencyId { get; set; }
    }
}
