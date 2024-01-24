using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Dtos.CryptoCurrencies;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Dtos.CryptoRewardSpendBands
{
    public class CryptoRewardSpendBandDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

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
        public BandType Type { get; set; }

        /// <summary>
        /// The created date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}
