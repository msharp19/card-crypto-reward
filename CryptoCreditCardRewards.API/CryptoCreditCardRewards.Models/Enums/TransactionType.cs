using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Enums
{
    public enum TransactionType
    {
        Reward = 0,
        Staking = 1,
        Deposit = 2,
        Withdrawal = 4,
        Fee = 8
    }
}
