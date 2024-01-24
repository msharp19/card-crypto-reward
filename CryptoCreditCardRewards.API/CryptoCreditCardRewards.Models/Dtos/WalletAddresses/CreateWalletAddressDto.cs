using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Dtos.WalletAddresses
{
    public class CreateWalletAddressDto
    {
        /// <summary>
        /// The currencys id the wallet address is for
        /// </summary>
        [JsonProperty("cryptoCurrencyId")]
        public int CryptoCurrencyId { get; set; }

        /// <summary>
        /// The user id the wallet address is for
        /// </summary>
        [JsonProperty("userId")]
        public int UserId { get; set; }
    }
}
