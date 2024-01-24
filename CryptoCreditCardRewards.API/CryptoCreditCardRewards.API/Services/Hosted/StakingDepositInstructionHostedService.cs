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
    public class StakingDepositInstructionHostedService : BaseHostedService<StakingDepositInstructionHostedService, StakingDepositInstructionHostedServiceSettings>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly StakingDepositInstructionHostedServiceSettings _settings;
        private IStakingDepositInstructionProcessorService _stakingDepositInstructionProcessorService;

        public StakingDepositInstructionHostedService(IServiceScopeFactory serviceScopeFactory, IOptions<StakingDepositInstructionHostedServiceSettings> settings,
            ILogger<StakingDepositInstructionHostedService> logger) : base(logger, settings)
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

            _stakingDepositInstructionProcessorService = scope.ServiceProvider.GetService<IStakingDepositInstructionProcessorService>();

            // Return the scope so it can be disposed elsewhere
            return scope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"StakingDepositInstructionService executed at: {DateTime.Now}");

            // Scope in the services
            using var serviceScope = GetScope();

            await base.ExecuteSafelyAsync(() => BackgroundJob.Enqueue(() => _stakingDepositInstructionProcessorService.ProcessStakingDepositInstructionsAsync()), CancellationToken.None);

            return;
        }
    }
}
