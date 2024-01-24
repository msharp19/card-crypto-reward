using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Blockchain
{
    public class Gas
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Result")]
        public Result Result { get; set; }
    }

    public class Result
    {
        [JsonProperty("LastBlock")]
        public long LastBlock { get; set; }

        [JsonProperty("SafeGasPrice")]
        public long SafeGasPrice { get; set; }

        [JsonProperty("ProposeGasPrice")]
        public long ProposeGasPrice { get; set; }

        [JsonProperty("FastGasPrice")]
        public long FastGasPrice { get; set; }

        [JsonProperty("suggestBaseFee")]
        public decimal suggestBaseFee { get; set; }

        [JsonProperty("gasUsedRatio")]
        public string gasUsedRatio { get; set; }
    }
}
