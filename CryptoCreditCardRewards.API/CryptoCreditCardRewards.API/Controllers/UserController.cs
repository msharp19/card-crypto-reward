using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CryptoCreditCardRewards.API.Helpers;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos;
using CryptoCreditCardRewards.Models.Dtos.Accounts;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Utilities;

namespace CryptoCreditCardRewards.API.Controllers
{
    [ApiController]
    [Route("v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserCreateService _userCreateService;
        private readonly IUserUpdateService _userUpdateService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserCreateService userCreateService, IUserUpdateService userUpdateService, IUserService userService,
            IMapper mapper, ILogger<UserController> logger)
        {
            _userCreateService = userCreateService;
            _userUpdateService = userUpdateService;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="model">The user to create</param>
        /// <remarks>
        /// 
        /// </remarks>
        /// <returns>The created user</returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<UserDto>> CreateUserAsync([FromBody] CreateUserDto model)
        {
            // Try to create new account
            var newAccount = await _userCreateService.CreateUserAsync(model.Email, model.AccountNumber, model.CompletedKyc, true);

            return Ok(_mapper.Map<UserDto>(newAccount));
        }

        /// <summary>
        /// Deactivate a user
        /// </summary>
        /// <param name="id">The account to deactivate</param>
        /// <remarks>
        /// 
        /// </remarks>
        /// <returns>Ok</returns>
        [HttpPatch]
        [Route("{id}/deactivate")]
        public async Task<ActionResult> DeactivateUserAsync([FromRoute] int id)
        {
            // Deactivate
            await _userUpdateService.DeactivateUserAsync(id);

            return Ok();
        }

        /// <summary>
        /// Activate a user
        /// </summary>
        /// <param name="id">The user to activate</param>
        /// <remarks>
        /// 
        /// </remarks>
        /// <returns>The activated user</returns>
        [HttpPatch]
        [Route("{id}/activate")]
        public async Task<ActionResult<UserDto>> ReactivateUserAsync([FromRoute] int id)
        {
            // Deactivate
            var updatedUser = await _userUpdateService.ReactivateUserAsync(id);

            return Ok(_mapper.Map<UserDto>(updatedUser));
        }

        /// <summary>
        /// Complete KYC for a user (if not already completed ie. created with not complete)
        /// </summary>
        /// <param name="id">The user to complete kyc for</param>
        /// <returns>The user with completed kyc</returns>
        [HttpPatch]
        [Route("{id}/kyc")]
        public async Task<ActionResult<UserDto>> CompleteKycAsync([FromRoute] int id)
        {
            // Complete KYC
            var updatedUser = await _userUpdateService.CompleteKycAsync(id);

            return Ok(_mapper.Map<UserDto>(updatedUser));
        }

        /// <summary>
        /// Gets users paged
        /// </summary>
        /// <param name="search">A search term (searchs email + account number)</param>
        /// <param name="accountNumber">The users account number</param>
        /// <param name="email">The users email</param>
        /// <param name="state">The active state of the account</param>
        /// <param name="order">The order to display results in</param>
        /// <param name="orderProperty">The property to sort by</param>
        /// <param name="pageNumber">The page number to return (starts from 0)</param>
        /// <param name="perPage">The total results to return per page</param>
        /// <returns>Users paged</returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<PagedResultsDto<UserDto>>> GetAccountsPagedAsync([FromQuery] string? search, [FromQuery] string? orderProperty, 
            [FromQuery] string? email, [FromQuery] string? accountNumber, [FromQuery] bool? kycComplete, [FromQuery] ActiveState state = ActiveState.Active, 
            [FromQuery] Order order = Order.Ascending, [FromQuery] uint pageNumber = 1, [FromQuery] uint perPage = 30)
        {
            // Validate the order filter
            var validatedFilter = OrderHelper.ResolveOrderProperty(orderProperty, _userService.GetSortProperties, out orderProperty);

            // Get filtered data
            var users = _userService.GetUsersPaged(search, email, accountNumber, kycComplete, state, new Page((int)pageNumber, (int)perPage), new SortOrder(orderProperty, order));

            // Map and return
            return Ok(_mapper.Map<PagedResultsDto<UserDto>>(users));
        }

        /// <summary>
        /// Gets user by id
        /// </summary>
        /// <param name="id">The account to get</param>
        /// <returns>The user for the id</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<UserDto>> GetUserAsync([FromRoute] int id, [FromQuery] ActiveState state = ActiveState.Active)
        {
            // Get the account
            var account = _userService.GetUser(id, state);
            if (account == null)
                throw new NotFoundException(FailedReason.AccountDoesntExist, Property.Id);

            // Map and return
            return Ok(_mapper.Map<UserDto>(account));
        }
    }
}