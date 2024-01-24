using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;

namespace CryptoCreditCardRewards.Services.Functions.Interfaces
{
    public interface IStakingDepositInstructionProcessorService
    {
        /// <summary>
        /// Process staging deposit instructions (try to taker deposit and move to staking hot wallet).
        /// </summary>
        /// <returns>An async task</returns>
        Task ProcessStakingDepositInstructionsAsync();
    }
}
