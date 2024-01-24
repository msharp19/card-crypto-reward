using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using CryptoCreditCardRewards.Services.Functions;
using CryptoCreditCardRewards.Services.Functions.Interfaces;

namespace CryptoCreditCardRewards.CreditCardRewardIssuerFunction
{
    public class TransactionConfirmationProcessor
    {
        private readonly ITransactionConfirmationService _transactionConfirmationService;

        public TransactionConfirmationProcessor(ITransactionConfirmationService transactionConfirmationService)
        {
            _transactionConfirmationService = transactionConfirmationService;
        }

        [FunctionName("TransactionConfirmationProcessor")]
        public async Task Run([TimerTrigger("0 0 0 0 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"TransactionConfirmationProcessor Timer trigger function executed at: {DateTime.Now}");

            // Confirm and unconfirmed transactions
            await _transactionConfirmationService.ConfirmUnConfirmedTransactionsAsync();
        }
    }
}
