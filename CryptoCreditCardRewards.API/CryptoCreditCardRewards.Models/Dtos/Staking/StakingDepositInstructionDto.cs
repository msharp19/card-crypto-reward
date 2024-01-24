using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Dtos.Accounts;
using CryptoCreditCardRewards.Models.Dtos.WalletAddresses;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Dtos.Staking
{
    public class StakingDepositInstructionDto
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

        [JsonProperty("pickedUpDate")]
        public DateTime? PickedUpDate { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("account")]
        public UserDto Account { get; set; }

        [JsonProperty("walletAddress")]
        public WalletAddressDto WalletAddress { get; set; }
    }
}
