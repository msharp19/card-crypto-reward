using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Dtos.WhitelistedAddresses
{
    public class CreateWhitelistAddressDto
    {
        [JsonProperty("cryptoCurrencyId")]
        public int CryptoCurrencyId { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
