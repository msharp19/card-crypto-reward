using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CryptoCreditCardRewards.API.Helpers;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos;
using CryptoCreditCardRewards.Models.Dtos.Staking;
using CryptoCreditCardRewards.Models.Dtos.Transactions;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.API.Controllers
{
    [ApiController]
    [Route("v1/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionCreateService _transactionCreateService;
        private readonly ITransactionUpdateService _transactionUpdateService;
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionService transactionService, ITransactionUpdateService transactionUpdateService, ITransactionCreateService transactionCreateService, IMapper mapper)
        {
            _transactionService = transactionService;
            _transactionCreateService = transactionCreateService;
            _transactionUpdateService = transactionUpdateService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get users transactions
        /// </summary>
        /// <param name="userId">The transactions for the user</param>
        /// <param name="search">A search term (for crypto currency search)</param>
        /// <param name="walletAddressId">The wallet the transactions are for</param>
        /// <param name="transactionType">The type of transaction</param>
        /// <param name="orderProperty">The property to order results by</param>
        /// <param name="confirmedState">If the transactions have been confirmed or not</param>
        /// <param name="activeState">If the transaction is active or not</param>
        /// <param name="order">The order to return in</param>
        /// <param name="pageNumber">The page number</param>
        /// <param name="perPage">The number per page</param>
        /// <returns>A users transactions</returns>
        [HttpGet]
        [Route("user/{userId}")]
        public async Task<ActionResult<PagedResultsDto<TransactionDto>>> GetUsersTransactionsAsync([FromRoute] int userId, [FromQuery] string? search, [FromQuery] int? walletAddressId, [FromQuery] TransactionType? transactionType,
            [FromQuery] string orderProperty, [FromQuery] ConfirmedState confirmedState = ConfirmedState.Both, [FromQuery] ActiveState activeState = ActiveState.Active, [FromQuery] Order order = Order.Ascending,
            [FromQuery] uint pageNumber = 1, [FromQuery] uint perPage = 30)
        {
            // Validate the order filter
            var validatedFilter = OrderHelper.ResolveOrderProperty(orderProperty, _transactionService.GetSortProperties, out orderProperty);

            // Get the transactions
            var transactions = _transactionService.GetTransactionsPaged(userId, search, walletAddressId, transactionType, new Page((int)pageNumber, (int)perPage), new SortOrder(orderProperty, order));

            // Map and return
            return Ok(_mapper.Map<PagedResultsDto<TransactionDto>>(transactions));
        }

        /// <summary>
        /// Get a transaction
        /// </summary>
        /// <param name="id">The transaction get</param>
        /// <param name="state">The active state of the transaction</param>
        /// <returns>The transaction</returns>
        /// <exception cref="NotFoundException">Thrown if 
        ///       - Doesnt exist
        /// </exception>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransactionAsync([FromRoute] int id, [FromQuery] ActiveState state = ActiveState.Active)
        {
            // Get the transaction
            var transaction = _transactionService.GetTrasaction(id, state);
            if (transaction == null)
                throw new NotFoundException(FailedReason.TrasactionDoesntExist, Property.Id);

            // Map and return
            return Ok(_mapper.Map<TransactionDto>(transaction));
        }

        /// <summary>
        /// Review a transaction
        /// </summary>
        /// <param name="id">The transaction to review</param>
        /// <returns>The transaction</returns>
        /// <exception cref="NotFoundException">Thrown if 
        ///       - Doesnt exist
        /// </exception>
        [HttpPost]
        [Route("{id}/review")]
        public async Task<ActionResult<TransactionDto>> ReviewTransactionAsync([FromRoute] int id, [FromBody] ReviewTransactionDto model)
        {
            var reviewedTransaction = await _transactionUpdateService.ReviewTransactionAsync(id, model.Failed, model.FailedReason);

            // Map and return
            return Ok(_mapper.Map<TransactionDto>(reviewedTransaction));
        }
    }
}
