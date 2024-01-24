using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Services.Functions.Interfaces;

namespace CryptoCreditCardRewards.CreditCardRewardIssuerFunction
{
    public class StakingDepositInstructionProcessor
    {
        private readonly IStakingDepositInstructionProcessorService _stakingDepositInstructionProcessorService;

        public StakingDepositInstructionProcessor(IStakingDepositInstructionProcessorService stakingDepositInstructionProcessorService)
        {
            _stakingDepositInstructionProcessorService = stakingDepositInstructionProcessorService;
        }

        [FunctionName("StakingDepositInstructionProcessor")]
        public async Task Run([TimerTrigger("0 0 0 0 * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"StakingDepositInstructionProcessor Timer trigger function executed at: {DateTime.Now}");

            await _stakingDepositInstructionProcessorService.ProcessStakingDepositInstructionsAsync();
        }
    }
}
