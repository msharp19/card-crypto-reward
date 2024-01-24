using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Dtos.Transactions
{
    public class ReviewTransactionDto
    {
        [JsonProperty("failedReason")]
        public string FailedReason { get; set; }

        [JsonProperty("failed")]
        public bool Failed { get; set; }
    }
}
