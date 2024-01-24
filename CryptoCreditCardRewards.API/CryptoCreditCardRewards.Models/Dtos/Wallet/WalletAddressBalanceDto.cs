using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Dtos.Wallet
{
    public class WalletAddressBalanceDto
    {
        [JsonProperty("spendableBalance")]
        public decimal SpendableBalance { get; set; }

        [JsonProperty("outstandingInstructionBalance")]
        public decimal OutstandingInstructionBalance { get; set; }

        [JsonProperty("unconfirmedBalance")]
        public decimal UnconfirmedBalance { get; set; }

        [JsonProperty("unReviewedBalance")]
        public decimal UnReviewedBalance { get; set; }

        [JsonProperty("confirmedBalance")]
        public decimal ConfirmedBalance { get; set; }

        [JsonProperty("successfullyReviewedBalance")]
        public decimal SuccessfullyReviewedBalance { get; set; }

        [JsonProperty("unsuccessfullyReviewedBalance")]
        public decimal UnsuccessfullyReviewedBalance { get; set; }

        [JsonProperty("spendableStakedBalance")]
        public decimal SpendableStakedBalance { get; set; }

        [JsonProperty("unconfirmedStakedBalance")]
        public decimal UnconfirmedStakedBalance { get; set; }

        [JsonProperty("outstandingInstructionStakedBalance")]
        public decimal OutstandingInstructionStakedBalance { get; set; }
    }
}
