using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Functions.Interfaces;
using CryptoCreditCardRewards.Services.Helpers.Interfaces;
using CryptoCreditCardRewards.Services.Http.Interfaces;
using CryptoCreditCardRewards.Utilities;

namespace CryptoCreditCardRewards.Services.Functions
{
    public class CreditCardRewardIssuanceService : ICreditCardRewardIssuanceService
    {
        private readonly ITransactionService _transactionService;
        private readonly ICryptoRewardBandsService _cryptoRewardBandsService;
        private readonly IInstructionService _instructionService;
        private readonly ICreditCardTransactionService _creditCardTransactionService;
        private readonly IConversionProviderFactory _conversionProviderFactory;
        private readonly IUserService _userService;
        private readonly IStakingService _stakingService;

        public CreditCardRewardIssuanceService(ICreditCardTransactionService creditCardTransactionService, IUserService userService,
            IInstructionService instructionService, ICryptoRewardBandsService cryptoRewardBandsService, IConversionProviderFactory conversionProviderFactory,
            ITransactionService transactionService, IStakingService stakingService)
        {
            _creditCardTransactionService = creditCardTransactionService;
            _conversionProviderFactory = conversionProviderFactory;
            _stakingService = stakingService;
            _userService = userService;
            _instructionService = instructionService;
            _cryptoRewardBandsService = cryptoRewardBandsService;
            _transactionService = transactionService;
        }

        /// <summary>
        /// Issues reward instructions. 
        /// If an instruction for the period has already been created, it will not be added again
        /// </summary>
        /// <returns>An async task</returns>
        public async Task IssueRewardInstructionsAsync()
        {
            // Get all the active users accounts to reward
            var users = _userService.GetUsersPaged(null, null, null, true, ActiveState.Active, new Page(0, 99999), new SortOrder("createdDate", Order.Descending));

            // Get the from and to dates for last month
            (var fromDate, var toDate) = DateTimeUtility.GetLastMonth();

            // Create a job to issue the reward instruction 
            foreach (var user in users.Items)
            {
                // Create instruction
                var doesUserInstructionExistForThisPeriodAlready = _instructionService.DoesUserInstructionExistForThisPeriodAlready(user.Id, fromDate, toDate);
                if (!doesUserInstructionExistForThisPeriodAlready)
                {
                    // Get aggregate value of credit card (spend) transactions for the user
                    var aggregateSpendValueInHKD = await _creditCardTransactionService.GetAggregateTransactionValueAsync(fromDate, toDate, user.AccountNumber);

                    // Get aggregate value of staked transactions for the user
                    var aggregateStakeAmountInHKD = await _stakingService.GetStakingAggregateInCurrencyAsync(fromDate, toDate, user.Id, "HKD");

                    // Get the amount to reward from spend + staking
                    var reward = _cryptoRewardBandsService.GetRewardTotal(aggregateSpendValueInHKD.Amount, aggregateStakeAmountInHKD);

                    // Create instruction to reward
                    await _instructionService.CreateMonthlyRewardInstructionAsync(user.Id, fromDate, toDate, reward);
                }
            }

            return;
        }
    }
}
