using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Dtos.WhitelistedAddresses
{
    public class ValidateWhitelistAddressDto
    {
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("failedReason")]
        public string? FailedReason { get; set; }
    }
}
