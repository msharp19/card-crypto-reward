using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Dtos.Accounts;
using CryptoCreditCardRewards.Models.Dtos.CryptoCurrencies;

namespace CryptoCreditCardRewards.Models.Dtos.WalletAddresses
{
    public class WalletAddressDto
    {
        /// <summary>
        /// The wallet address id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// If the wallet address is active
        /// </summary>
        [JsonProperty("active")]
        public bool Active { get; set; }

        /// <summary>
        /// The actual address (public key)
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

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

        /// <summary>
        /// The currency the wallet address is for
        /// </summary>
        [JsonProperty("cryptoCurrency")]
        public CryptoCurrencyDto CryptoCurrency { get; set; }

        /// <summary>
        /// The user the wallet address is for
        /// </summary>
        [JsonProperty("user")]
        public UserDto User { get; set; }

        /// <summary>
        /// The created date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}
