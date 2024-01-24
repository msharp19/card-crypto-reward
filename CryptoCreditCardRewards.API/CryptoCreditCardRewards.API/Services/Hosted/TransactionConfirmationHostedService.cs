using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Functions;
using CryptoCreditCardRewards.Services.Functions.Interfaces;

namespace CryptoCreditCardRewards.API.Services.Hosted
{
    public class TransactionConfirmationHostedService  : BaseHostedService<TransactionConfirmationHostedService, TransactionConfirmationHostedServiceSettings>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TransactionConfirmationHostedServiceSettings _settings;
        private ITransactionConfirmationService _transactionConfirmationService;

        public TransactionConfirmationHostedService(IServiceScopeFactory serviceScopeFactory, IOptions<TransactionConfirmationHostedServiceSettings> settings,
            ILogger<TransactionConfirmationHostedService> logger) : base(logger, settings)
        {
            _settings = settings.Value;
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Setup all the required services to use
        /// </summary>
        public IServiceScope GetScope()
        {
            // Create the scope
            var scope = _serviceScopeFactory.CreateScope();

            _transactionConfirmationService = scope.ServiceProvider.GetService<ITransactionConfirmationService>();

            // Return the scope so it can be disposed elsewhere
            return scope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"TransactionConfirmationService executed at: {DateTime.Now}");

            // Scope in the services
            using var serviceScope = GetScope();

            // Confirm and unconfirmed transactions
            await base.ExecuteSafelyAsync(() => BackgroundJob.Enqueue(() => _transactionConfirmationService.ConfirmUnConfirmedTransactionsAsync()), CancellationToken.None);

            return;
        }
    }
}
