using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Services.Functions.Interfaces
{
    public interface IStakingWithdrawalInstructionProcessorService
    {
        Task ProcessStakingWithdrawalInstructionsAsync();
    }
}
