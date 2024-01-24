using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Misc
{
    public class StakingBalance
    {
        public decimal ConfirmedBalance { get; set; }
        public decimal UnconfirmedBalance { get; set; }

        public decimal TotalBalance => ConfirmedBalance + UnconfirmedBalance;
    }
}
