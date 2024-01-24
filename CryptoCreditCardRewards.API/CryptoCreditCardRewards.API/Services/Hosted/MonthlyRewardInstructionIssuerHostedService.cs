using Hangfire;
using Microsoft.Extensions.Options;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Functions.Interfaces;

namespace CryptoCreditCardRewards.API.Services.Hosted
{
    public class MonthlyRewardInstructionIssuerHostedService : BaseHostedService<MonthlyRewardInstructionIssuerHostedService, RewardInstructionIssuerHostedServiceSettings>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RewardInstructionIssuerHostedServiceSettings _settings;
        private ICreditCardRewardIssuanceService _creditCardRewardIssuanceService;

        public MonthlyRewardInstructionIssuerHostedService(IServiceScopeFactory serviceScopeFactory, IOptions<RewardInstructionIssuerHostedServiceSettings> settings,
            ILogger<MonthlyRewardInstructionIssuerHostedService> logger) : base(logger, settings)
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

            _creditCardRewardIssuanceService = scope.ServiceProvider.GetService<ICreditCardRewardIssuanceService>();

            // Return the scope so it can be disposed elsewhere
            return scope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"RewardInstructionIssuerHostedService executed at: {DateTime.Now}");

            // Scope in the services
            using var serviceScope = GetScope();

            // Issue reward instructions
            await base.ExecuteSafelyAsync(() => BackgroundJob.Enqueue(() => _creditCardRewardIssuanceService.IssueRewardInstructionsAsync()), CancellationToken.None);
            
            return;
        }
    }
}
