using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Dtos.Accounts;
using CryptoCreditCardRewards.Models.Dtos.CryptoCurrencies;

namespace CryptoCreditCardRewards.Models.Dtos.UserRewardSelections
{
    public class UserRewardSelectionDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("cryptoCurrencyId")]
        public int CryptoCurrencyId { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("contributionPercentage")]
        public decimal ContributionPercentage { get; set; }

        [JsonProperty("cryptoCurrency", NullValueHandling = NullValueHandling.Ignore)]
        public CryptoCurrencyDto? CryptoCurrency { get; set; }

        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public UserDto? User { get; set; }

        /// <summary>
        /// The created date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}
