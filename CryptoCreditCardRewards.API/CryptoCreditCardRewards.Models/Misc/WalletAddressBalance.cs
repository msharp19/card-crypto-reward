using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Misc
{
    public class WalletAddressBalance
    {
        public decimal SpendableBalance { get; set; }
        public decimal OutstandingInstructionBalance { get; set; }
        public decimal UnconfirmedBalance { get; set; }
        public decimal UnReviewedBalance { get; set; }
        public decimal ConfirmedBalance { get; set; }
        public decimal SuccessfullyReviewedBalance { get; set; }
        public decimal UnsuccessfullyReviewedBalance { get; set; }
        public decimal SpendableStakedBalance { get; set; }
        public decimal UnconfirmedStakedBalance { get; set; }
        public decimal OutstandingInstructionStakedBalance { get; set; }
    }
}
