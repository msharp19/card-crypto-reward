using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CryptoCreditCardRewards.Models.Dtos;
using CryptoCreditCardRewards.Models.Dtos.Accounts;
using CryptoCreditCardRewards.Models.Dtos.WalletAddresses;
using CryptoCreditCardRewards.Models.Dtos.UserRewardSelections;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Utilities;
using CryptoCreditCardRewards.API.Helpers;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Exceptions;

namespace CryptoCreditCardRewards.API.Controllers
{
    [ApiController]
    [Route("v1/reward-selections")]
    public class RewardSelectionController : ControllerBase
    {
        private readonly IUserRewardSelectionUpdateService _userRewardSelectionUpdateService;
        private readonly IUserRewardSelectionService _userRewardSelectionService;
        private readonly IMapper _mapper;
        private readonly ILogger<RewardSelectionController> _logger;

        public RewardSelectionController(IUserRewardSelectionUpdateService userRewardSelectionUpdateService, IUserRewardSelectionService userRewardSelectionService,
            IMapper mapper, ILogger<RewardSelectionController> logger)
        {
            _userRewardSelectionUpdateService = userRewardSelectionUpdateService;
            _userRewardSelectionService = userRewardSelectionService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Update selection reward structure. This takes the total (cannot submit one at a time, must contain all required)
        /// </summary>
        /// <param name="userId">The user id to set reward structure for</param>
        /// <returns>The updated reward selection/s</returns>
        [HttpPatch]
        [Route("user/{userId}")]
        public async Task<ActionResult<UserRewardSelectionDto>> UpdateRewardSelectionAsync([FromRoute] int userId, [FromBody] UpdateRewardSelectionsDto rewardSelectionDto)
        {
            // Map
            var rewardSelectionsToAdd = _mapper.Map<List<UserRewardSelection>>(rewardSelectionDto.RewardSelections);

            // Update
            var updatedRewardSelections = await _userRewardSelectionUpdateService.UpdateUserRewardSelectionsAsync(userId, rewardSelectionsToAdd);

            // Return
            return Ok(_mapper.Map<List<UserRewardSelectionDto>>(updatedRewardSelections));
        }

        /// <summary>
        /// Gets user reward selections paged
        /// </summary>
        /// <returns>User reward selctions</returns>
        [HttpGet]
        [Route("user/{userId}")]
        public async Task<ActionResult<PagedResultsDto<UserRewardSelectionDto>>> GetUserRewardSelectionsPagedAsync([FromRoute] int userId, [FromQuery] string? search,
            [FromQuery] int? cryptoCurrencyId, [FromQuery] string? orderProperty, [FromQuery] Order order = Order.Ascending,
            [FromQuery] uint pageNumber = 1, [FromQuery] uint perPage = 30)
        {
            // Validate the order filter
            var validatedFilter = OrderHelper.ResolveOrderProperty(orderProperty, _userRewardSelectionService.GetSortProperties, out orderProperty);

            // Get filtered data
            var rewardSelections = _userRewardSelectionService.GetUserRewardSelectionsPaged(userId, search, cryptoCurrencyId, new Page((int)pageNumber, (int)perPage), new SortOrder(orderProperty, order));

            // Map and return
            return Ok(_mapper.Map<PagedResultsDto<UserRewardSelectionDto>>(rewardSelections));
        }

        /// <summary>
        /// Get a users reward selection for a wallet
        /// </summary>
        /// <param name="id">The id of the reward selection</param>
        /// <param name="state">The state of the reward selection</param>
        /// <returns>A users reward selection</returns>
        /// <exception cref="NotFoundException">Thrown if
        ///       - The reward selection doesn't exist
        /// </exception>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<UserRewardSelectionDto>> GetUserRewardSelectionAsync([FromRoute] int id)
        {
            // Get the reward selection
            var rewardSelection = _userRewardSelectionService.GetRewardSelection(id);
            if (rewardSelection == null)
                throw new NotFoundException(FailedReason.UserRewardSelectionDoesntExist, Property.Id);

            // Map and return
            return Ok(_mapper.Map<UserRewardSelectionDto>(rewardSelection));
        }
    }
}