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
    public class RewardPaymentInstructionHostedService : BaseHostedService<RewardPaymentInstructionHostedService, RewardPaymentInstructionHostedServiceSettings>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RewardPaymentInstructionHostedServiceSettings _settings;
        private IRewardPaymentInstructionProcessingService _rewardPaymentInstructionProcessingService;

        public RewardPaymentInstructionHostedService(IServiceScopeFactory serviceScopeFactory, IOptions<RewardPaymentInstructionHostedServiceSettings> settings,
            ILogger<RewardPaymentInstructionHostedService> logger) : base(logger, settings)
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

            _rewardPaymentInstructionProcessingService = scope.ServiceProvider.GetService<IRewardPaymentInstructionProcessingService>();

            // Return the scope so it can be disposed elsewhere
            return scope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"RewardPaymentInstructionHostedService executed at: {DateTime.Now}");

            // Scope in the services
            using var serviceScope = GetScope();

            // Process payment instructions
            await base.ExecuteSafelyAsync(() => BackgroundJob.Enqueue(() => _rewardPaymentInstructionProcessingService.ProcessRewardPaymentInstructionsAsync()), CancellationToken.None);

            return;
        }
    }
}
