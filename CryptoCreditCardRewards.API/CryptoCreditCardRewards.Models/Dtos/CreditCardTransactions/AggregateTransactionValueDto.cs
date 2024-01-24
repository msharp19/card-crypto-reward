using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Dtos.CreditCardTransactions
{
    public class AggregateTransactionValueDto
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("cryptoCurrencyId")]
        public int? CryptoCurrencyId { get; set; }
    }
}
