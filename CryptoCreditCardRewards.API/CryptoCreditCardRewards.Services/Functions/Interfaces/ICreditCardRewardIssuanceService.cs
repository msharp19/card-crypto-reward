using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Services.Functions.Interfaces
{
    public interface ICreditCardRewardIssuanceService
    {
        /// <summary>
        /// Issues reward instructions. 
        /// If an instruction for the period has already been created, it will not be added again
        /// </summary>
        /// <returns>An async task</returns>
        Task IssueRewardInstructionsAsync();
    }
}
