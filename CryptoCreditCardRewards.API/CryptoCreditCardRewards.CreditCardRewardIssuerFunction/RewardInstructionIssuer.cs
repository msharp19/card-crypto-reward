using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using CryptoCreditCardRewards.Services.Functions;
using CryptoCreditCardRewards.Services.Functions.Interfaces;

namespace CryptoCreditCardRewards.CreditCardRewardIssuerFunction
{
    public class RewardInstructionIssuer
    {
        private readonly ICreditCardRewardIssuanceService _creditCardRewardIssuanceService;

        public RewardInstructionIssuer(ICreditCardRewardIssuanceService creditCardRewardIssuanceService)
        {
            _creditCardRewardIssuanceService = creditCardRewardIssuanceService;
        }

        [FunctionName("RewardInstructionIssuer")]
        public async Task Run([TimerTrigger("0 0 0 0 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"RewardInstructionIssuer Timer trigger function executed at: {DateTime.Now}");

            // Issue reward instructions
            await _creditCardRewardIssuanceService.IssueRewardInstructionsAsync();
        }
    }
}
