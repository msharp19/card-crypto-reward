using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Services.Functions.Interfaces
{
    public interface IWithdrawalInstructionProcessorService
    {
        /// <summary>
        /// Process withdrawal instructions (try to move from staking hot wallet to users wallet).
        /// </summary>
        /// <returns>An async task</returns>
        Task ProcessWithdrawalInstructionsAsync();
    }
}
