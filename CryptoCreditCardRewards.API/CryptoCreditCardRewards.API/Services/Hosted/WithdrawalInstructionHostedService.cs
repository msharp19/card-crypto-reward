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
    /// <summary>
    /// Service to process withdraw instructions
    /// </summary>
    public class WithdrawalInstructionHostedService : BaseHostedService<WithdrawalInstructionHostedService, WithdrawalInstructionHostedServiceSettings>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly WithdrawalInstructionHostedServiceSettings _settings;
        private IWithdrawalInstructionProcessorService _withdrawalInstructionProcessorService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public WithdrawalInstructionHostedService(IServiceScopeFactory serviceScopeFactory, IOptions<WithdrawalInstructionHostedServiceSettings> settings,
            ILogger<WithdrawalInstructionHostedService> logger) : base(logger, settings)
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

            _withdrawalInstructionProcessorService = scope.ServiceProvider.GetService<IWithdrawalInstructionProcessorService>();

            // Return the scope so it can be disposed elsewhere
            return scope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"WithdrawalInstructionService executed at: {DateTime.Now}");

            // Scope in the services
            using var serviceScope = GetScope();

            await base.ExecuteSafelyAsync(() => BackgroundJob.Enqueue(() => _withdrawalInstructionProcessorService.ProcessWithdrawalInstructionsAsync()), CancellationToken.None);

            return;
        }
    }
}
