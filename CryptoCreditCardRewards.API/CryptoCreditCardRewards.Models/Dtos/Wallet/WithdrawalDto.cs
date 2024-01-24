using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Dtos.Wallet
{
    public class WithdrawalDto
    {
        [JsonProperty("whitelistAddressIdTo")]
        public int WhitelistAddressIdTo { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// The created date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}
