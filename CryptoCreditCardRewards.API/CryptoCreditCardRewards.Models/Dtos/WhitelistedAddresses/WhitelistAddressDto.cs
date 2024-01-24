using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Dtos.CryptoCurrencies;

namespace CryptoCreditCardRewards.Models.Dtos.WhitelistedAddresses
{
    public class WhitelistAddressDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("processedDate")]
        public DateTime? ProcessedDate { get; set; }

        [JsonProperty("valid")]
        public bool Valid { get; set; }

        [JsonProperty("failedReason")]
        public string? FailedReason { get; set; }

        [JsonProperty("cryptoCurrencyId")]
        public int CryptoCurrencyId { get; set; }

        [JsonProperty("cryptoCurrency")]
        public CryptoCurrencyDto CryptoCurrency { get; set; }

        /// <summary>
        /// The created date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}
