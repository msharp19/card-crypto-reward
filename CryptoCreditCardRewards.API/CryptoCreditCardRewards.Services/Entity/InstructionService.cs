using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Contexts;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos.CreditCardTransactions;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Http.Interfaces;

namespace CryptoCreditCardRewards.Services.Entity
{
    public class InstructionService : IInstructionService
    {
        private readonly CryptoCreditCardRewardsDbContext _context;

        public InstructionService(CryptoCreditCardRewardsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks if a user already has an instruction for a specified date range 
        /// </summary>
        /// <param name="userId">The user to check for</param>
        /// <param name="fromDate">The from date</param>
        /// <param name="toDate">The to date</param>
        /// <returns>True if a user has an instruction for the date range provided</returns>
        public bool DoesUserInstructionExistForThisPeriodAlready(int userId, DateTime fromDate, DateTime toDate)
        {
            return _context.Instructions.Where(x => x.UserId == userId)
                .Where(x => x.FromDate == fromDate)
                .Where(x => x.ToDate == toDate)
                .Where(x => x.Type == InstructionType.MonthlyReward)
                .Any();
        }

        /// <summary>
        /// Create a monthly reward instruction
        /// </summary>
        /// <param name="userId">The user to create the instruction for</param>
        /// <param name="fromDate">When the instruction is from</param>
        /// <param name="toDate">When instruction is to</param>
        /// <param name="transactionValue">The value to reward user</param>
        /// <returns>A montly rewaard instruction</returns>
        public async Task<Instruction> CreateMonthlyRewardInstructionAsync(int userId, DateTime fromDate, DateTime toDate, decimal transactionValue)
        {
            var instruction = new Instruction(true, transactionValue, userId, null, null, null, fromDate, toDate, 1, 0, 0);
            instruction.SetType(InstructionType.MonthlyReward);

            _context.Instructions.Add(instruction);
            await _context.SaveChangesAsync();

            return instruction;
        }

        /// <summary>
        /// Create a deposit for staking instruction
        /// </summary>
        /// <param name="userId">The user to create the instruction for</param>
        /// <param name="date">When the instruction is for</param>
        /// <param name="transactionValue">The value to stake (Native Currency ie. ETH)</param>
        /// <returns>A staking deposit instruction</returns>
        public async Task<Instruction> CreateStakingDepositInstructionAsync(int userId, int walletAddressId, DateTime date, decimal transactionValue, decimal fee, decimal feeToUse)
        {
            var instruction = new Instruction(true, transactionValue, userId, walletAddressId, null, null, date, date, 1, fee, feeToUse);
            instruction.SetType(InstructionType.StakingDeposit);

            _context.Instructions.Add(instruction);
            await _context.SaveChangesAsync();

            return instruction;
        }

        /// <summary>
        /// Create a withdraw for staking instruction
        /// </summary>
        /// <param name="userId">The user to create the instruction for</param>
        /// <param name="date">When the instruction is for</param>
        /// <param name="transactionValue">The stake (Native Currency ie. ETH)</param>
        /// <returns>A staking withdrawal instruction</returns>
        public async Task<Instruction> CreateStakingWithdrawalInstructionAsync(int userId, int walletAddressId, DateTime date, decimal transactionValue, decimal fee, decimal feeToUse)
        {
            var instruction = new Instruction(true, -1 * transactionValue, userId, walletAddressId, null, null, date, date, 1, fee, feeToUse);
            instruction.SetType(InstructionType.StakingWithdrawal);

            _context.Instructions.Add(instruction);
            await _context.SaveChangesAsync();

            return instruction;
        }

        /// <summary>
        /// Create a withdraw instruction
        /// </summary>
        /// <param name="userId">The user to create the instruction for</param>
        /// <param name="date">When the instruction is for</param>
        /// <param name="transactionValue">The value to withdraw</param>
        /// <returns>A withdrawal instruction</returns>
        public async Task<Instruction> CreateWithdrawalInstructionAsync(int userId, int walletAddressId, int? whitelistedAddressTo, DateTime date, decimal transactionValue, decimal fee, decimal feeToUse)
        {
            var instruction = new Instruction(true, -1 * transactionValue, userId, walletAddressId, null, whitelistedAddressTo, date, date, 1, fee, feeToUse);
            instruction.SetType(InstructionType.Withdrawal);

            _context.Instructions.Add(instruction);
            await _context.SaveChangesAsync();

            return instruction;
        }

        /// <summary>
        /// Get an instruction
        /// </summary>
        /// <param name="instructionId">The instruction to get</param>
        /// <param name="state">The state of the instruction</param>
        /// <returns>An instruction if it exists</returns>
        public Instruction? GetInstruction(int instructionId, ActiveState state)
        {
            // Get the instruction to pickup
            var instructions = _context.Instructions.Include(x => x.User)
                .Include(x => x.WhitelistAddress)
                .Include(x => x.WalletAddress)
                .Include(x => x.ParentInstruction)
                .Where(x => x.Id == instructionId);

            // Filter by active state
            if (state == ActiveState.Active)
                instructions = instructions.Where(x => x.Active);
            else if (state == ActiveState.InActive)
                instructions = instructions.Where(x => !x.Active);

            return instructions.FirstOrDefault();
        }

        /// <summary>
        /// Pickup an instruction to process
        /// </summary>
        /// <param name="paymentInstructionId">The payment instruction to pick up</param>
        /// <returns>A picked up payment instruction</returns>
        public async Task<Instruction> PickupInstructionToProcessAsync(int paymentInstructionId)
        {
            // Get the instruction to pickup
            var instruction = _context.Instructions.FirstOrDefault(x => x.Id == paymentInstructionId);

            // Pickup
            instruction.PickUp();

            // Save
            _context.Instructions.Update(instruction);
            await _context.SaveChangesAsync();

            return instruction;
        }

        /// <summary>
        /// Fail an instruction
        /// </summary>
        /// <param name="instructionId">The instruction to fail</param>
        /// <param name="failedReason">The reason the instruction failed</param>
        /// <returns>The failed instruction</returns>
        public async Task<Instruction> FailInstructionAsync(int instructionId, string failedReason)
        {
            // Get the instruction to pickup
            var instruction = _context.Instructions.FirstOrDefault(x => x.Id == instructionId);

            // Pickup
            instruction.Fail(failedReason);

            // Save
            _context.Instructions.Update(instruction);
            await _context.SaveChangesAsync();

            return instruction;
        }

        /// <summary>
        /// Put back an instruction (to be picked up at a later date)
        /// </summary>
        /// <param name="paymentInstructionId">The payment instruction to pick up</param>
        /// <returns>An instruction waiting for pickup</returns>
        public async Task<Instruction> PutBackInstructionToProcessLaterAsync(int paymentInstructionId)
        {
            // Get the instruction to pickup
            var instruction = _context.Instructions.FirstOrDefault(x => x.Id == paymentInstructionId);

            // Pickup
            instruction.PutBack();

            // Save
            _context.Instructions.Update(instruction);
            await _context.SaveChangesAsync();

            return instruction;
        }

        /// <summary>
        /// Get a list of reward payment instructions to process (complete payment for)
        /// </summary>
        /// <returns>Reward payment instructions that require filling</returns>
        public List<Instruction> GetRewardPaymentInstructionsToProcess()
        {
            return _context.Instructions.Where(x => x.Type == InstructionType.RewardPayment)
                .Where(x => x.Active)
                .Where(x => x.PickedUpDate == null)
                .ToList();
        }

        /// <summary>
        /// Get a list of monthly reward instructions to process (create payment instructions for)
        /// </summary>
        /// <returns>Monthly Reward instructions that require payment instruction creation</returns>
        public List<Instruction> GetMonthlyRewardInstructionsToProcess()
        {
            return _context.Instructions.Where(x => x.Type == InstructionType.MonthlyReward)
                .Where(x => x.Active)
                .Where(x => x.PickedUpDate == null)
                .ToList();
        }

        /// <summary>
        /// Get a list of staking deposit instructions to process (complete payment for)
        /// </summary>
        /// <returns>Staking deposit instructions that require filling</returns>
        public List<Instruction> GetStakingDepositInstructionsToProcess()
        {
            return _context.Instructions.Where(x => x.Type == InstructionType.StakingDeposit)
                .Where(x => x.Active)
                .Where(x => x.PickedUpDate == null)
                .ToList();
        }

        /// <summary>
        /// Get a list of staking withdrawal instructions to process (complete payment for)
        /// </summary>
        /// <returns>Staking withdrawal instructions that require filling</returns>
        public List<Instruction> GetStakingWithdrawalInstructionsToProcess()
        {
            return _context.Instructions.Where(x => x.Type == InstructionType.StakingWithdrawal)
                .Where(x => x.Active)
                .Where(x => x.PickedUpDate == null)
                .ToList();
        }

        /// <summary>
        /// Add a set of instructions
        /// </summary>
        /// <param name="instructions">The instructions to add</param>
        /// <returns>The added instrcutions</returns>
        public async Task<List<Instruction>> AddInstructionsAsync(List<Instruction> instructions)
        {
            _context.Instructions.AddRange(instructions);
            await _context.SaveChangesAsync();

            return instructions;
        }

        /// <summary>
        /// Get a list of withdrawal instructions to process (complete payment for)
        /// </summary>
        /// <returns>Withdrawal instructions that require filling</returns>
        public List<Instruction> GetWithdrawalInstructionsToProcess()
        {
            return _context.Instructions.Where(x => x.Type == InstructionType.Withdrawal)
                .Where(x => x.Active)
                .Where(x => x.PickedUpDate == null)
                .ToList();
        }

        /// <summary>
        /// Gets the value of outstanding instructions (not completed)
        /// </summary>
        /// <param name="walletAddressId">The wallet address to get unconfirmed balance for</param>
        /// <param name="instructionType">The type of instruction</param>
        /// <returns>The unprocessed value of a set of instructions for a wallet</returns>
        public decimal GetUnProcessedInstructionsValue(int walletAddressId, InstructionType instructionType)
        {
            return _context.Instructions.Where(x => x.WalletAddressId == walletAddressId)
                .Where(x => x.Active)
                .Where(x => x.Type == instructionType)
                .Where(x => x.CompletedDate == null)
                .Sum(x => x.Amount + x.MonetaryFee);
        }

        /// <summary>
        /// Complete an instruction
        /// </summary>
        /// <param name="paymentInstructionId">The id of the instruction to complete</param>
        /// <returns>The completed instruction</returns>
        public async Task<Instruction> CompleteInstructionAsync(int paymentInstructionId)
        {
            // Get the instruction to pickup
            var instruction = _context.Instructions.FirstOrDefault(x => x.Id == paymentInstructionId);

            // Pickup
            instruction.SetComplete();

            // Save
            _context.Instructions.Update(instruction);
            await _context.SaveChangesAsync();

            return instruction;
        }

        /// <summary>
        /// Gets the properties to sort by
        /// </summary>
        /// <returns>All properties to sort by</returns>
        public List<(string PropertyName, Order Order)> GetSortProperties()
        {
            return new List<(string PropertyName, Order Order)>()
            {
                new ("createdDate", Order.Ascending),
                new ("createdDate", Order.Descending),
                new ("amount", Order.Ascending),
                new ("amount", Order.Descending),
            };
        }

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
        public PagedResults<Instruction> GetInstructionsPaged(int? userId, string? search, InstructionType? type, int? walletAddressId, decimal? fromAmount,
            decimal? toAmount, int? parentInstructionId, ActiveState state, Page page, SortOrder sortOrder)
        {
            // Get instructions query
            var instructions = GetInstructionsQuery(userId, search, type, walletAddressId, fromAmount, toAmount, parentInstructionId, state);

            // Sort 
            instructions = OrderInstructions(instructions, sortOrder);

            // Paginate
            var results = instructions.Skip((int)(page.PageIndex * page.PerPage))
                .Take((int)page.PerPage)
                .ToList();

            // Get total
            var total = GetInstructionsQuery(userId, search, type, walletAddressId, fromAmount, toAmount, parentInstructionId, state).Select(x => x.Id).Count();

            // Map and return
            return new PagedResults<Instruction>()
            {
                Items = results,
                Page = page,
                SortOrder = sortOrder,
                TotalCount = total
            };
        }

        #region Helpers

        private IQueryable<Instruction> GetInstructionsQuery(int? userId, string? search, InstructionType? type, int? walletAddressId, decimal? fromAmount, decimal? toAmount,
            int? parentInstructionId, ActiveState state)
        {
            var instructions = _context.Instructions.Include(x => x.ParentInstruction)
                .Include(x => x.User)
                .Include(x => x.WhitelistAddress)
                .Include(x => x.WalletAddress)
                .ThenInclude(x => x.CryptoCurrency)
                .AsQueryable();

            // Filter by user
            if (userId.HasValue)
            {
                instructions = instructions.Where(x => x.UserId == userId);
            }

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().Trim();
                instructions = instructions.Where(x => x.WalletAddress != null)
                    .Where(x => x.WalletAddress.CryptoCurrency != null)
                    .Where(x => x.WalletAddress.CryptoCurrency.Name.Contains(search) ||
                            x.WalletAddress.CryptoCurrency.Symbol.Contains(search));
            }

            // Filter by type
            if (type.HasValue)
            {
                instructions = instructions.Where(x => x.Type == type);
            }

            // Filter by wallet address
            if (walletAddressId.HasValue)
            {
                instructions = instructions.Where(x => x.WalletAddressId == walletAddressId);
            }

            // Filter by from amount
            if (fromAmount.HasValue)
            {
                instructions = instructions.Where(x => x.Amount >= fromAmount);
            }

            // Filter by to amount
            if (toAmount.HasValue)
            {
                instructions = instructions.Where(x => x.Amount <= toAmount);
            }

            // Filter by parent
            if (parentInstructionId.HasValue)
            {
                instructions = instructions.Where(x => x.ParentInstructionId == parentInstructionId);
            }

            // Filter by active state
            if (state == ActiveState.Active)
                instructions = instructions.Where(x => x.Active);
            else if (state == ActiveState.InActive)
                instructions = instructions.Where(x => !x.Active);

            return instructions;
        }

        /// <summary>
        /// Orders instructions in a queryable list
        /// </summary>
        /// <param name="instructions">The list to order</param>
        /// <param name="sortOrder">The sort order details</param>
        /// <returns>Sorted instruction list</returns>
        private IQueryable<Instruction> OrderInstructions(IQueryable<Instruction> instructions, SortOrder sortOrder)
        {
            // Sort users where supported property exists - default name
            return (sortOrder.OrderProperty.Trim(), sortOrder.Order) switch
            {
                ("createdDate", Order.Ascending) => instructions.OrderBy(x => x.CreatedDate),
                ("createdDate", Order.Descending) => instructions.OrderByDescending(x => x.CreatedDate),
                ("amount", Order.Ascending) => instructions.OrderBy(x => x.Amount),
                ("amount", Order.Descending) => instructions.OrderByDescending(x => x.Amount),
                _ => sortOrder.Order == Order.Ascending ? instructions.OrderBy(x => x.CreatedDate) : instructions.OrderByDescending(x => x.CreatedDate)
            };
        }

        #endregion
    }
}
