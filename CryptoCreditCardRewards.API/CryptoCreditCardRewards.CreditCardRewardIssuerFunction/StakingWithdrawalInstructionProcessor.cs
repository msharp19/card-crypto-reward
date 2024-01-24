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
    public class StakingWithdrawalInstructionProcessor
    {
        private readonly IStakingWithdrawalInstructionProcessorService _stakingWithdrawalInstructionProcessorService;

        public StakingWithdrawalInstructionProcessor(IStakingWithdrawalInstructionProcessorService stakingWithdrawalInstructionProcessorService)
        {
            _stakingWithdrawalInstructionProcessorService = stakingWithdrawalInstructionProcessorService;
        }

        [FunctionName("StakingWithdrawalInstructionProcessor")]
        public async Task Run([TimerTrigger("0 0 0 0 * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"StakingWithdrawalInstructionProcessor Timer trigger function executed at: {DateTime.Now}");

            await _stakingWithdrawalInstructionProcessorService.ProcessStakingWithdrawalInstructionsAsync();
        }
    }
}
