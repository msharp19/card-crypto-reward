using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using CryptoCreditCardRewards.Services.Functions;
using CryptoCreditCardRewards.Services.Functions.Interfaces;

namespace CryptoCreditCardRewards.CreditCardRewardIssuerFunction
{
    public class RewardPaymentInstructionProcessor
    {
        private readonly IRewardPaymentInstructionProcessingService _rewardPaymentInstructionProcessingService;

        public RewardPaymentInstructionProcessor(IRewardPaymentInstructionProcessingService paymentInstructionProcessingService)
        {
            _rewardPaymentInstructionProcessingService = paymentInstructionProcessingService;
        }

        [FunctionName("PaymentInstructionProcessor")]
        public async Task Run([TimerTrigger("0 0 0 0 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"RewardPaymentInstructionProcessor Timer trigger function executed at: {DateTime.Now}");

            // Process payment instructions
            await _rewardPaymentInstructionProcessingService.ProcessRewardPaymentInstructionsAsync();
        }
    }
}
