using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Contexts;
using CryptoCreditCardRewards.Services.Blockchain;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Entity;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Functions;
using CryptoCreditCardRewards.Services.Functions.Interfaces;
using CryptoCreditCardRewards.Services.Http;
using CryptoCreditCardRewards.Services.Http.Interfaces;

[assembly: FunctionsStartup(typeof(CryptoCreditCardRewards.CreditCardRewardIssuerFunction.Startup))]
namespace CryptoCreditCardRewards.CreditCardRewardIssuerFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            ExecutionContextOptions executionContextOptions = builder.Services.BuildServiceProvider().GetService<IOptions<ExecutionContextOptions>>().Value;

            var config = new ConfigurationBuilder()
                .SetBasePath(executionContextOptions.AppDirectory)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("local.settings.json", true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .Build();

            builder.Services.SetContext<CryptoCreditCardRewardsDbContext>(config.GetConnectionString("ApplicationDatabase"));

            builder.Services.AddTransient<IBlockchainProviderFactory, BlockchainProviderFactory>();

            builder.Services.AddTransient<IInstructionService, InstructionService>();
            builder.Services.AddTransient<ICreditCardTransactionService, CreditCardTransactionService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<ITransactionService, TransactionService>();
            builder.Services.AddTransient<ICryptoRewardBandsService, CryptoRewardBandsService>();

            builder.Services.AddTransient<ICreditCardRewardIssuanceService, CreditCardRewardIssuanceService>();
            builder.Services.AddTransient<IRewardPaymentInstructionProcessingService, RewardPaymentInstructionProcessingService>();
            builder.Services.AddTransient<IStakingDepositInstructionProcessorService, StakingDepositInstructionProcessorService>();
            builder.Services.AddTransient<IStakingWithdrawalInstructionProcessorService, StakingWithdrawalInstructionProcessorService>();
            builder.Services.AddTransient<ITransactionConfirmationService, TransactionConfirmationService>();
        }
    }
}
