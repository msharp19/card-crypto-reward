using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CryptoCreditCardRewards.API.Helpers;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos;
using CryptoCreditCardRewards.Models.Dtos.Accounts;
using CryptoCreditCardRewards.Models.Dtos.Staking;
using CryptoCreditCardRewards.Models.Dtos.Transactions;
using CryptoCreditCardRewards.Models.Dtos.Wallet;
using CryptoCreditCardRewards.Models.Dtos.WalletAddresses;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Utilities;

namespace CryptoCreditCardRewards.API.Controllers
{
    [ApiController]
    [Route("v1/wallets")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletCreateService _walletCreateService;
        private readonly IWalletAddressService _walletAddressService;
        private readonly ICryptoWithdrawalService _cryptoWithdrawalService;
        private readonly ITransactionCreateService _transactionCreateService;
        private readonly IStakingCreateService _stakingCreateService;
        private readonly IStakingDeleteService _stakingDeleteService;
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletCreateService walletCreateService, IWalletAddressService walletAddressService, ICryptoWithdrawalService cryptoWithdrawalService,
            ITransactionCreateService transactionCreateService, ITransactionService transactionService, IMapper mapper,
            IStakingCreateService stakingCreateService, IStakingDeleteService stakingDeleteService, ILogger<WalletController> logger)
        {
            _walletAddressService = walletAddressService;
            _cryptoWithdrawalService = cryptoWithdrawalService;
            _walletCreateService = walletCreateService;
            _transactionService = transactionService;
            _transactionCreateService = transactionCreateService;
            _stakingCreateService = stakingCreateService;
            _stakingDeleteService = stakingDeleteService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Create a wallet address for an account
        /// </summary>
        /// <param name="model">The currency to create wallet for</param>
        /// <returns>The created wallet</returns>
        [HttpPost]
        [Route("address")]
        public async Task<ActionResult<WalletAddressDto>> CreateWalletAddressAsync([FromBody] CreateWalletAddressDto model)
        {
            // Try to add new wallet address
            var newWalletAddress = await _walletCreateService.AddWalletAddressToUserAsync(model.UserId, model.CryptoCurrencyId);

            // Return
            return Ok(_mapper.Map<WalletAddressDto>(newWalletAddress));
        }

        /// <summary>
        /// Gets addresses
        /// </summary>
        /// <param name="userId">The user the address is for</param>
        /// <param name="search">A search term</param>
        /// <param name="cryptoCurrencyId">The crypto currency</param>
        /// <param name="state">The state of the wallet</param>
        /// <param name="orderProperty">The property or order results by</param>
        /// <param name="order">The order of the results</param>
        /// <param name="pageNumber">The page to return</param>
        /// <param name="perPage">The number of results per page</param>
        /// <returns>users addresses</returns>
        [HttpGet]
        [Route("addresses")]
        public async Task<ActionResult<PagedResultsDto<WalletAddressDto>>> GetUsersAddressesPagedAsync([FromQuery] int? userId, [FromQuery] string? search, 
            [FromQuery] int? cryptoCurrencyId,[FromQuery] ActiveState? state, [FromQuery] string? orderProperty, [FromQuery] Order order = Order.Ascending,
            [FromQuery] uint pageNumber = 1, [FromQuery] uint perPage = 30)
        {
            // Validate the order filter
            var validatedFilter = OrderHelper.ResolveOrderProperty(orderProperty, _walletAddressService.GetSortProperties, out orderProperty);

            // Get filtered data
            var wallets = _walletAddressService.GetWalletAddressesPaged(search, userId, cryptoCurrencyId, state, new Page((int)pageNumber, (int)perPage), new SortOrder(orderProperty, order));

            // Map and return
            return Ok(_mapper.Map<PagedResultsDto<WalletAddressDto>>(wallets));
        }

        /// <summary>
        /// Get the balance of a wallet address
        /// </summary>
        /// <param name="walletAddressId">The wallet address to get balance of</param>
        /// <returns>Balance of wallet address</returns>
        [HttpGet]
        [Route("address/{walletAddressId}/balance")]
        public async Task<ActionResult<WalletAddressBalanceDto>> GetWalletAddressBalanceAsync([FromRoute] int walletAddressId)
        {
            var walletBalance = _transactionService.GetBalance(walletAddressId);

            return Ok(_mapper.Map<WalletAddressBalanceDto>(walletBalance));
        }

        /// <summary>
        /// Gets the total stake balance for a user. This returns a breakdown of the confirm/unconfirmed transaction values
        /// </summary>
        /// <param name="userId">The user to get total staked for</param>
        /// <param name="walletAddressId">Optional wallet address</param>
        /// <returns>A confirmed and unconfirmed balance for a users staked value</returns>
        [HttpGet]
        [Route("user/{userId}/stake/balance")]
        public async Task<ActionResult<StakingBalanceDto>> GetUsersAmountStakedAsync([FromRoute] int userId, [FromQuery] int? walletAddressId)
        {
            // Get the balance
            var stakingBalance = _transactionService.AggregateTransactionValue(TransactionType.Staking, userId, walletAddressId);

            // Map and return
            return Ok(_mapper.Map<StakingBalanceDto>(stakingBalance));
        }

        /// <summary>
        /// Get a wallet address
        /// </summary>
        /// <param name="id">The wallet address to get</param>
        /// <param name="state">The active state of the wallet</param>
        /// <returns>The wallet address</returns>
        /// <exception cref="NotFoundException">Thrown if 
        ///       - Doesnt exist
        /// </exception>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<WalletAddressDto>> GetWalletAddressAsync([FromRoute] int id, [FromQuery] ActiveState state = ActiveState.Active)
        {
            // Get the wallet address
            var walletAddress = _walletAddressService.GetWalletAddress(id, state);
            if (walletAddress == null)
                throw new NotFoundException(FailedReason.WalletAddressDoesntExist, Property.Id);

            // Map and return
            return Ok(_mapper.Map<WalletAddressDto>(walletAddress));
        }

        /// <summary>
        /// Create an instruction to initiate a crypto wallet address withdrawal
        /// </summary>
        /// <param name="walletAddressId">The wallet to initiate the request for</param>
        /// <param name="model">The data required to initiate a withdrawal</param>
        /// <returns>Ok</returns>
        [HttpPost]
        [Route("address/{walletAddressId}/withdraw")]
        public async Task<ActionResult> InitiateWithdrawalAsync([FromRoute] int walletAddressId, [FromBody] WithdrawalDto model)
        {
            await _cryptoWithdrawalService.WithdrawWalletBalanceAsync(walletAddressId, model.WhitelistAddressIdTo, model.Amount);

            return Ok();
        }

        /// <summary>
        /// Create an instruction to initiate a crypto wallet address deposit
        /// </summary>
        /// <param name="walletAddressId">The wallet address for the deposit to</param>
        /// <param name="hash">The transaction to import</param>
        /// <returns>An imported deposit transaction</returns>
        [HttpPost]
        [Route("address/{walletAddressId}/deposit/{hash}")]
        public async Task<ActionResult<TransactionDto>> CreateDepositTransactionAsync([FromRoute] int walletAddressId, [FromRoute] string hash)
        {
            // Get the transaction
            var transaction = await _transactionCreateService.CreateDepositTransactionAsync(walletAddressId, hash);

            // Map and return
            return Ok(_mapper.Map<TransactionDto>(transaction));
        }

        /// <summary>
        /// Create an instruction to initiate the process to stake an amount X
        /// </summary>
        /// <param name="walletAddressId">The wallet address to stake for</param>
        /// <param name="model">The data required to stake</param>
        /// <returns>An instruction for the initiation of the staking process</returns>
        [HttpPost]
        [Route("address/{walletAddressId}/stake")]
        public async Task<ActionResult> StakeAsync([FromRoute] int walletAddressId, [FromBody] StakingOptionsDto model)
        {
            // Create the instruction
            var stakingInstruction = await _stakingCreateService.StakeCurrencyAsync(walletAddressId, model.Amount);

            // Map and return
            return Ok(_mapper.Map<StakingDepositInstructionDto>(stakingInstruction));
        }

        /// <summary>
        /// Create an instruction to initiate the removal X of some currency staked
        /// </summary>
        /// <param name="walletAddressId">The wallet address where the user has value staked</param>
        /// <param name="model">The data required to initiate the removal of an amount staked</param>
        /// <returns>An instruction for the withdrawal of the staking process</returns>
        [HttpPatch]
        [Route("address/{walletAddressId}/stake")]
        public async Task<ActionResult> RemoveStakeAsync([FromRoute] int walletAddressId, [FromBody] StakingOptionsDto model)
        {
            // Create the instruction
            var stakingInstruction = await _stakingDeleteService.UnStakeCurrencyAsync(walletAddressId, model.Amount);

            // Map and return
            return Ok(_mapper.Map<StakingWithdrawalInstructionDto>(stakingInstruction));
        }
    }
}