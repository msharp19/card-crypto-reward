using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Dtos.Staking
{
    public class StakingBalanceDto
    {
        [JsonProperty("confirmedBalance")]
        public decimal ConfirmedBalance { get; set; }

        [JsonProperty("unconfirmedBalance")]
        public decimal UnconfirmedBalance { get; set; }

        [JsonProperty("totalBalance")]
        public decimal TotalBalance { get; set; }
    }
}
