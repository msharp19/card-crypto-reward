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
    public class WithdrawalInstructionProcessor
    {
        private readonly IWithdrawalInstructionProcessorService _withdrawalInstructionProcessorService;

        public WithdrawalInstructionProcessor(IWithdrawalInstructionProcessorService withdrawalInstructionProcessorService)
        {
            _withdrawalInstructionProcessorService = withdrawalInstructionProcessorService;
        }

        [FunctionName("WithdrawalInstructionProcessor")]
        public async Task Run([TimerTrigger("0 0 0 0 * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"WithdrawalInstructionProcessor Timer trigger function executed at: {DateTime.Now}");

            await _withdrawalInstructionProcessorService.ProcessWithdrawalInstructionsAsync();
        }
    }
}
