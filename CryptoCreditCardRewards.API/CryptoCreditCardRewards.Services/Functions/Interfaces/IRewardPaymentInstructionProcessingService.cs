using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Services.Functions.Interfaces
{
    public interface IRewardPaymentInstructionProcessingService
    {
        /// <summary>
        /// Processes all reward payment instructions of type "RewardPayment".
        /// These are reward payments to users based on their spend/reward selection
        /// </summary>
        /// <returns>An async task</returns>
        Task ProcessRewardPaymentInstructionsAsync();
    }
}
