using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Enums
{
    public enum InstructionType
    {
        MonthlyReward = 0,
        RewardPayment = 1,
        Withdrawal = 2,
        StakingDeposit = 3,
        StakingWithdrawal = 4,
    }
}
