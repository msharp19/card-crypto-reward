using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Dtos.Accounts;
using CryptoCreditCardRewards.Models.Dtos.CryptoCurrencies;
using CryptoCreditCardRewards.Models.Dtos.SystemWallets;
using CryptoCreditCardRewards.Models.Dtos.WalletAddresses;
using CryptoCreditCardRewards.Models.Dtos.WhitelistedAddresses;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Dtos.Transactions
{
    public class TransactionDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("cryptoCurrencyId")]
        public int CryptoCurrencyId { get; set; }

        [JsonProperty("type")]
        public TransactionType Type { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("state")]
        public TransactionState State { get; set; }

        [JsonProperty("confirmedDate")]
        public DateTime? ConfirmedDate { get; set; }

        [JsonProperty("fromAddress")]
        public string? FromAddress { get; set; }

        [JsonProperty("toAddress")]
        public string? ToAddress { get; set; }

        [JsonProperty("walletAddressId")]
        public int WalletAddressId { get; set; }

        [JsonProperty("systemWalletAddressId")]
        public int SystemWalletAddressId { get; set; }

        [JsonProperty("whitelistAddressId")]
        public int WhitelistAddressId { get; set; }

        [JsonProperty("reviewedDate")]
        public DateTime? ReviewedDate { get; set; }

        [JsonProperty("reviewedNotes")]
        public string? ReviewedNotes { get; set; }

        [JsonProperty("failedReview")]
        public bool FailedReview { get; set; }

        [JsonProperty("whitelistAddress")]
        public WhitelistAddressDto? WhitelistAddress { get; set; }

        [JsonProperty("systemWalletAddress")]
        public SystemWalletAddressDto? SystemWalletAddress { get; set; }

        [JsonProperty("walletAddress")]
        public WalletAddressDto WalletAddress { get; set; }

        [JsonProperty("cryptoCurrency")]
        public CryptoCurrencyDto CryptoCurrency { get; set; }

        [JsonProperty("user")]
        public UserDto User { get; set; }

        /// <summary>
        /// The created date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

    }
}
