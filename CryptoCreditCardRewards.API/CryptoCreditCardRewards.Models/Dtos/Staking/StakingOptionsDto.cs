using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Dtos.Staking
{
    public class StakingOptionsDto
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
