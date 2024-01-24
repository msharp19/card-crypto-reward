using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CryptoCreditCardRewards.API.Helpers;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos;
using CryptoCreditCardRewards.Models.Dtos.CryptoRewardSpendBands;
using CryptoCreditCardRewards.Models.Dtos.Staking;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.API.Controllers
{
    [ApiController]
    [Route("v1/reward-spend-bands")]
    public class RewardSpendBandController : ControllerBase
    {
        private readonly ICryptoRewardBandCreateService _cryptoRewardBandCreateService;
        private readonly ICryptoRewardBandsService _cryptoRewardBandsService;
        private readonly ICryptoRewardBandDeleteService _cryptoRewardBandDeleteService;
        private readonly IMapper _mapper;

        public RewardSpendBandController(ICryptoRewardBandsService cryptoRewardBandsService, ICryptoRewardBandCreateService cryptoRewardBandCreateService, ICryptoRewardBandDeleteService cryptoRewardBandDeleteService,
            IMapper mapper)
        {
            _cryptoRewardBandsService = cryptoRewardBandsService;
            _cryptoRewardBandCreateService = cryptoRewardBandCreateService;
            _cryptoRewardBandDeleteService = cryptoRewardBandDeleteService;
            _mapper = mapper;
        }

        /// <summary>
        /// Create a reward spend band
        /// </summary>
        /// <param name="model">The reward spend band to add</param>
        /// <returns>The created reward spend band</returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<CryptoRewardSpendBandDto>> CreateRewardSpendBandAsync([FromBody] CreateCryptoRewardSpendBandDto model)
        {
            // Create
            var rewardSpendBand = await _cryptoRewardBandCreateService.CreateRewardSpendBandAsync(model.BandType, model.Name, model.Description, model.BandFrom, model.BandTo, model.PercentageReward, model.CryptoCurrencyId);

            // Return
            return Ok(_mapper.Map<CryptoRewardSpendBandDto>(rewardSpendBand));
        }

        /// <summary>
        /// Gets crypto reward spend bands paged
        /// </summary>
        /// <param name="search">Name/description</param>
        /// <param name="bandType">The type of band</param>
        /// <param name="fromBandFrom">From band from value</param>
        /// <param name="toBandFrom">To band from value</param>
        /// <param name="fromBandTo">From band to value</param>
        /// <param name="toBandTo">To band to value</param>
        /// <param name="fromPercentageReward">From percentage reward value</param>
        /// <param name="toPercentageReward">To percentage reward value</param>
        /// <param name="orderProperty">The property to order by</param>
        /// <param name="fromUpTo">From up to value</param>
        /// <param name="toUpTo">To up to value</param>
        /// <param name="order">The order to return the results</param>
        /// <param name="pageNumber">The page to be returned</param>
        /// <param name="perPage">The amount per page to be returned</param>
        /// <returns>Crypto reward spend bands</returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<PagedResultsDto<CryptoRewardSpendBandDto>>> GetCrypoRewardSpendBandsPagedAsync([FromQuery] string? search, [FromQuery] BandType? bandType, [FromQuery] decimal? fromBandFrom, 
            [FromQuery] decimal? toBandFrom, [FromQuery] decimal? fromBandTo, [FromQuery] decimal? toBandTo, [FromQuery] decimal? fromPercentageReward, [FromQuery] decimal? toPercentageReward,
            [FromQuery] string? orderProperty, [FromQuery] decimal? fromUpTo, [FromQuery] decimal? toUpTo, [FromQuery] Order order = Order.Ascending, [FromQuery] uint pageNumber = 1, [FromQuery] uint perPage = 30)
        {
            // Validate the order filter
            var validatedFilter = OrderHelper.ResolveOrderProperty(orderProperty, _cryptoRewardBandsService.GetSortProperties, out orderProperty);
 
            // Get filtered data
            var rewardSelections = _cryptoRewardBandsService.GetCryptoRewardSpendBandsPaged(search, fromBandFrom, fromBandTo, toBandFrom, toBandTo, fromPercentageReward, toPercentageReward, 
                fromUpTo, toUpTo, bandType, new Page((int)pageNumber, (int)perPage), new SortOrder(orderProperty, order));

            // Map and return
            return Ok(_mapper.Map<PagedResultsDto<CryptoRewardSpendBandDto>>(rewardSelections));
        }

        /// <summary>
        /// Get a reward spend band
        /// </summary>
        /// <param name="id">The reward spend band to get</param>
        /// <param name="state">The active state of the spend band</param>
        /// <returns>The reward spend band</returns>
        /// <exception cref="NotFoundException">Thrown if 
        ///       - Doesnt exist
        /// </exception>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CryptoRewardSpendBandDto>> GetRewardSpendBandAsync([FromRoute] int id, [FromQuery] ActiveState state = ActiveState.Active)
        {
            // Get the reward spend band
            var rewardSpendBand = _cryptoRewardBandsService.GetRewardBand(id, state);
            if (rewardSpendBand == null)
                throw new NotFoundException(FailedReason.RewardSpendBandDoesntExist, Property.Id);

            // Map and return
            return Ok(_mapper.Map<CryptoRewardSpendBandDto>(rewardSpendBand));
        }

        /// <summary>
        /// Delete a reward spend band
        /// </summary>
        /// <param name="id">The reward spend band to delete</param>
        /// <returns>An async task</returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<CryptoRewardSpendBandDto>> DeleteRewardSpendBandAsync([FromRoute] int id)
        {
            // Delete the reward spend band
            await _cryptoRewardBandDeleteService.DeleteRewardBandAsync(id);

            return Ok();
        }
    }
}
