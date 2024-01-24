using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Dtos.UserRewardSelections;

namespace CryptoCreditCardRewards.Models.Entities
{
    public class UpdateRewardSelectionsDto
    {
        [JsonProperty("rewardSelections")]
        public List<UpdateRewardSelectionDto> RewardSelections { get; set; }
    }
}
