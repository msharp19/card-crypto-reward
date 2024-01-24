using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Dtos.Accounts;
using CryptoCreditCardRewards.Models.Dtos.WalletAddresses;
using CryptoCreditCardRewards.Models.Dtos.WhitelistedAddresses;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Dtos.Instructions
{
    public class InstructionDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("type")]
        public InstructionType Type { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("walletAddressId")]
        public int? WalletAddressId { get; set; }

        [JsonProperty("whitelistAddressId")]
        public int? WhitelistAddressId { get; set; }

        [JsonProperty("pickedUpDate")]
        public DateTime? PickedUpDate { get; set; }

        [JsonProperty("completedDate")]
        public DateTime? CompletedDate { get; set; }

        [JsonProperty("parentInstructionId")]
        public int? ParentInstructionId { get; set; }

        [JsonProperty("failedDate")]
        public DateTime? FailedDate { get; set; }

        [JsonProperty("failedReason")]
        public string? FailedReason { get; set; }

        [JsonProperty("fromDate")]
        public DateTime FromDate { get; set; }

        [JsonProperty("toDate")]
        public DateTime ToDate { get; set; }


        [JsonProperty("user")]
        public UserDto User;

        [JsonProperty("walletAddress")]
        public WalletAddressDto? WalletAddress;

        [JsonProperty("whitelistAddress")]
        public WhitelistAddressDto? WhitelistAddress;

        [JsonProperty("parentInstruction")]
        public InstructionDto? ParentInstruction { get; set; }

        /// <summary>
        /// The created date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}
