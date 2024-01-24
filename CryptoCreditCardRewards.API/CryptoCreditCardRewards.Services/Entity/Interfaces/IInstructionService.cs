using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos.CreditCardTransactions;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Services.Entity.Interfaces
{
    public interface IInstructionService
    {
        /// <summary>
        /// Checks if a user already has an instruction for a specified date range 
        /// </summary>
        /// <param name="userId">The user to check for</param>
        /// <param name="fromDate">The from date</param>
        /// <param name="toDate">The to date</param>
        /// <returns>True if a user has an instruction for the date range provided</returns>
        bool DoesUserInstructionExistForThisPeriodAlready(int userId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get an instruction
        /// </summary>
        /// <param name="instructionId">The instruction to get</param>
        /// <param name="state">The state of the instruction</param>
        /// <returns>An instruction if it exists</returns>
        Instruction? GetInstruction(int instructionId, ActiveState state);

        /// <summary>
        /// Create a monthly reward instruction
        /// </summary>
        /// <param name="userId">The user to create the instruction for</param>
        /// <param name="fromDate">When the instruction is from</param>
        /// <param name="toDate">When instruction is to</param>
        /// <param name="transactionValue">The value to reward user (HKD)</param>
        /// <returns>A montly rewaard instruction</returns>
        Task<Instruction> CreateMonthlyRewardInstructionAsync(int userId, DateTime fromDate, DateTime toDate, decimal transactionValue);
       
        /// <summary>
        /// Gets the properties to sort by
        /// </summary>
        /// <returns>All properties to sort by</returns>
        List<(string PropertyName, Order Order)> GetSortProperties();

        /// <summary>
        /// Create a deposit for staking instruction
        /// </summary>
        /// <param name="userId">The user to create the instruction for</param>
        /// <param name="date">When the instruction is for</param>
        /// <param name="transactionValue">The value to stake (Native Currency ie. ETH)</param>
        /// <returns>A staking deposit instruction</returns>
        Task<Instruction> CreateStakingDepositInstructionAsync(int userId, int walletAddressId, DateTime date, decimal transactionValue, decimal fee, decimal feeToUse);

        /// <summary>
        /// Create a withdraw for staking instruction
        /// </summary>
        /// <param name="userId">The user to create the instruction for</param>
        /// <param name="date">When the instruction is for</param>
        /// <param name="transactionValue">The stake (Native Currency ie. ETH)</param>
        /// <returns>A staking withdrawal instruction</returns>
        Task<Instruction> CreateStakingWithdrawalInstructionAsync(int userId, int walletAddressId, DateTime date, decimal transactionValue, decimal fee, decimal feeToUse);

        /// <summary>
        /// Create a withdraw instruction
        /// </summary>
        /// <param name="userId">The user to create the instruction for</param>
        /// <param name="date">When the instruction is for</param>
        /// <param name="transactionValue">The value to withdraw</param>
        /// <returns>A withdrawal instruction</returns>
        Task<Instruction> CreateWithdrawalInstructionAsync(int userId, int walletAddressId, int? whitelistedAddressTo, DateTime date, decimal transactionValue, decimal fee, decimal feeToUse);

        /// <summary>
        /// Pickup an instruction to process
        /// </summary>
        /// <param name="paymentInstructionId">The payment instruction to pick up</param>
        /// <returns>A picked up payment instruction</returns>
        Task<Instruction> PickupInstructionToProcessAsync(int paymentInstructionId);

        /// <summary>
        /// Put back an instruction (to be picked up at a later date)
        /// </summary>
        /// <param name="paymentInstructionId">The payment instruction to pick up</param>
        /// <returns>An instruction waiting for pickup</returns>
        Task<Instruction> PutBackInstructionToProcessLaterAsync(int paymentInstructionId);

        /// <summary>
        /// Get a list of monthly reward instructions to process (create payment instructions for)
        /// </summary>
        /// <returns>Monthly Reward instructions that require payment instruction creation</returns>
        List<Instruction> GetMonthlyRewardInstructionsToProcess();

        /// <summary>
        /// Get a list of reward payment instructions to process (complete payment for)
        /// </summary>
        /// <returns>Reward payment instructions that require filling</returns>
        List<Instruction> GetRewardPaymentInstructionsToProcess();

        /// <summary>
        /// Get a list of staking deposit instructions to process (complete payment for)
        /// </summary>
        /// <returns>Staking deposit instructions that require filling</returns>
        List<Instruction> GetStakingDepositInstructionsToProcess();

        /// <summary>
        /// Get a list of staking withdrawal instructions to process (complete payment for)
        /// </summary>
        /// <returns>Staking withdrawal instructions that require filling</returns>
        List<Instruction> GetStakingWithdrawalInstructionsToProcess();

        /// <summary>
        /// Add a set of instructions
        /// </summary>
        /// <param name="instructions">The instructions to add</param>
        /// <returns>The added instrcutions</returns>
        Task<List<Instruction>> AddInstructionsAsync(List<Instruction> instructions);

        /// <summary>
        /// Gets the value of outstanding instructions (not completed)
        /// </summary>
        /// <param name="walletAddressId">The wallet address to get unconfirmed balance for</param>
        /// <param name="instructionType">The type of instruction</param>
        /// <returns>The unprocessed value of a set of instructions for a wallet</returns>
        decimal GetUnProcessedInstructionsValue(int walletAddressId, InstructionType instructionType);

        /// <summary>
        /// Get a list of withdrawal instructions to process (complete payment for)
        /// </summary>
        /// <returns>Withdrawal instructions that require filling</returns>
        List<Instruction> GetWithdrawalInstructionsToProcess();

        /// <summary>
        /// Complete an instruction
        /// </summary>
        /// <param name="paymentInstructionId">The id of the instruction to complete</param>
        /// <returns>The completed instruction</returns>
        Task<Instruction> CompleteInstructionAsync(int paymentInstructionId);

        /// <summary>
        /// Fail an instruction
        /// </summary>
        /// <param name="instructionId">The instruction to fail</param>
        /// <param name="failedReason">The reason the instruction failed</param>
        /// <returns>The failed instruction</returns>
        Task<Instruction> FailInstructionAsync(int instructionId, string failedReason);

        /// <summary>
        /// Get instructions paged for a user
        /// </summary>
        /// <param name="userId">The user the instructions are for</param>
        /// <param name="search">A search term</param>
        /// <param name="type">The type of instruction</param>
        /// <param name="walletAddressId">The wallet address for instruction</param>
        /// <param name="fromAmount">The amount from</param>
        /// <param name="toAmount">The amount to</param>
        /// <param name="parentInstructionId">The parent of the instruction</param>
        /// <param name="state">The state of the instruction</param>
        /// <param name="page">The page to return</param>
        /// <param name="sortOrder">The order of the page to return</param>
        /// <returns>Paged instructions</returns>
        PagedResults<Instruction> GetInstructionsPaged(int? userId, string? search, InstructionType? type, int? walletAddressId, decimal? fromAmount,
            decimal? toAmount, int? parentInstructionId, ActiveState state, Page page, SortOrder sortOrder);
    }
}
