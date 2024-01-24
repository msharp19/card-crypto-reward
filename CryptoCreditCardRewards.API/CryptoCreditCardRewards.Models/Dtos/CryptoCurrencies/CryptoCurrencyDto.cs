using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Dtos.CryptoCurrencies
{
    public class CryptoCurrencyDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("nName")]
        public string Name { get; set; }

        [JsonProperty("networkEndpoint")]
        public string NetworkEndpoint { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("isTestNetwork")]
        public bool IsTestNetwork { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("supportsStaking")]
        public bool SupportsStaking { get; set; }

        [JsonProperty("infrastructureType")]
        public InfrastructureType InfrastructureType { get; set; }

        /// <summary>
        /// The created date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}
