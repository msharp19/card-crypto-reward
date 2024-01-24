using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Dtos.UserRewardSelections
{
    public class UpdateRewardSelectionDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("contributionPercentage")]
        public decimal ContributionPercentage { get; set; }
    }
}
