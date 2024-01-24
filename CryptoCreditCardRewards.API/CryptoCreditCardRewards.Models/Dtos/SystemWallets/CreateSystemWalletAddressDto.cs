using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Dtos.SystemWallets
{
    public class CreateSystemWalletAddressDto
    {
        /// <summary>
        /// The type of address
        /// </summary>
        [JsonProperty("addressType")]
        public AddressType AddressType { get; set; }

        /// <summary>
        /// The currencys id the wallet address is for
        /// </summary>
        [JsonProperty("cryptoCurrencyId")]
        public int CryptoCurrencyId { get; set; }
    }
}
