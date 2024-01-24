using Hangfire;
using Microsoft.Extensions.Options;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.EventHandlers;
using CryptoCreditCardRewards.Services.Functions.Interfaces;

namespace CryptoCreditCardRewards.API.Services.Hosted
{
    public class RewardPaymentInstructionIssuerHostedService : BaseHostedService<RewardPaymentInstructionIssuerHostedService, RewardPaymentInstructionIssuerHostedServiceSettings>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RewardPaymentInstructionIssuerHostedServiceSettings _settings;
        private IMonthlyRewardInstructionIssuerService _monthlyRewardInstructionIssuerService;

        public RewardPaymentInstructionIssuerHostedService(IServiceScopeFactory serviceScopeFactory, IOptions<RewardPaymentInstructionIssuerHostedServiceSettings> settings,
            ILogger<RewardPaymentInstructionIssuerHostedService> logger) : base(logger, settings)
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

            _monthlyRewardInstructionIssuerService = scope.ServiceProvider.GetService<IMonthlyRewardInstructionIssuerService>();

            // Return the scope so it can be disposed elsewhere
            return scope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"RewardPaymentInstructionIssuerHostedService executed at: {DateTime.Now}");

            // Scope in the services
            using var serviceScope = GetScope();

            // Issue reward instructions
            await base.ExecuteSafelyAsync(() => BackgroundJob.Enqueue(() => _monthlyRewardInstructionIssuerService.ProcessMonthlyRewardInstructionsAsync()), CancellationToken.None);
            
            return;
        }
    }
}
