using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Functions.Interfaces;

namespace CryptoCreditCardRewards.API.Services.Hosted
{
    public class StakingWithdrawalInstructionHostedService : BaseHostedService<StakingWithdrawalInstructionHostedService, StakingWithdrawalInstructionHostedServiceSettings>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly StakingWithdrawalInstructionHostedServiceSettings _settings;
        private IStakingWithdrawalInstructionProcessorService _stakingWithdrawalInstructionProcessorService;

        public StakingWithdrawalInstructionHostedService(IServiceScopeFactory serviceScopeFactory, IOptions<StakingWithdrawalInstructionHostedServiceSettings> settings,
            ILogger<StakingWithdrawalInstructionHostedService> logger) : base(logger, settings)
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

            _stakingWithdrawalInstructionProcessorService = scope.ServiceProvider.GetService<IStakingWithdrawalInstructionProcessorService>();

            // Return the scope so it can be disposed elsewhere
            return scope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"StakingWithdrawalInstructionService executed at: {DateTime.Now}");

            // Scope in the services
            using var serviceScope = GetScope();

            await base.ExecuteSafelyAsync(() => BackgroundJob.Enqueue(() => _stakingWithdrawalInstructionProcessorService.ProcessStakingWithdrawalInstructionsAsync()), CancellationToken.None);

            return;
        }
    }
}
