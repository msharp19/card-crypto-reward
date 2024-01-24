using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CryptoCreditCardRewards.API.Helpers;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos;
using CryptoCreditCardRewards.Models.Dtos.Accounts;
using CryptoCreditCardRewards.Models.Dtos.Wallet;
using CryptoCreditCardRewards.Models.Dtos.WalletAddresses;
using CryptoCreditCardRewards.Models.Dtos.WhitelistedAddresses;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Services.Helpers.Interfaces;
using CryptoCreditCardRewards.Utilities;

namespace CryptoCreditCardRewards.API.Controllers
{
    [ApiController]
    [Route("v1/whitelist-address")]
    public class WhitelistAddressController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<WhitelistAddressController> _logger;
        private readonly IWhitelistAddressService _whitelistAddressService;
        private readonly IWhitelistAddressCreateService _whitelistAddressCreateService;
        private readonly IWhitelistAddressUpdateService _whitelistAddressUpdateService;
        private readonly IStakingService _stakingService;

        public WhitelistAddressController(IWhitelistAddressService whitelistAddressService, IWhitelistAddressCreateService whitelistAddressCreateService,
            IWhitelistAddressUpdateService whitelistAddressUpdateService, IStakingService stakingService, IMapper mapper, ILogger<WhitelistAddressController> logger)
        {
            _whitelistAddressCreateService = whitelistAddressCreateService;
            _whitelistAddressUpdateService = whitelistAddressUpdateService;
            _stakingService = stakingService;
            _whitelistAddressService = whitelistAddressService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get a whitelisted address
        /// </summary>
        /// <param name="id">The whitelisted address get</param>
        /// <returns>The whitelisted address</returns>
        /// <exception cref="NotFoundException">Thrown if 
        ///       - Doesnt exist
        /// </exception>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<WhitelistAddressDto>> GetWhitelistAddressAsync([FromRoute] int id)
        {
            // Get the white list address
            var whitelistAddress = _whitelistAddressService.GetWhitelistAddress(id, WhitelistAddressState.Any);
            if (whitelistAddress == null)
                throw new NotFoundException(FailedReason.WhitelistAddressDoesntExist, Property.Id);

            // Map and return
            return Ok(_mapper.Map<WhitelistAddressDto>(whitelistAddress));
        }

        /// <summary>
        /// Gets white list addresses
        /// </summary>
        /// <param name="search">A search term</param>
        /// <param name="vaildated"></param>
        /// <param name="processedDateFrom">From date the address has been processed (validated) at</param>
        /// <param name="processedDateTo">To date the address has been processed (validated) at</param>
        /// <param name="cryptoCurrencyId">The crypto currency</param>
        /// <param name="userId">The user the address is whitelisted for</param>
        /// <param name="orderProperty">The property or order results by</param>
        /// <param name="address">The whitelist address</param>
        /// <param name="order">The order of the results</param>
        /// <param name="pageNumber">The page to return</param>
        /// <param name="perPage">The number of results per page</param>
        /// <returns>Whitelisted addresses</returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<PagedResultsDto<WhitelistAddressDto>>> GetWhitelistedAddressesPagedAsync([FromQuery] int? userId, [FromQuery] string? search, [FromQuery]bool? vaildated,
            [FromQuery] DateTime? processedDateFrom, [FromQuery] DateTime? processedDateTo, [FromQuery] int? cryptoCurrencyId, [FromQuery] string? address, [FromQuery] string? orderProperty,
            [FromQuery] Order order = Order.Ascending, [FromQuery] uint pageNumber = 1, [FromQuery] uint perPage = 30)
        {
            // Validate the order filter
            var validatedFilter = OrderHelper.ResolveOrderProperty(orderProperty, _whitelistAddressService.GetSortProperties, out orderProperty);

            // Get filtered data
            var addresses = _whitelistAddressService.GetWhitelistAddressesPaged(search, userId, cryptoCurrencyId, vaildated, processedDateFrom, processedDateTo, address,
                new Page((int)pageNumber, (int)perPage), new SortOrder(orderProperty, order));

            // Map and return
            return Ok(_mapper.Map<PagedResultsDto<WhitelistAddressDto>>(addresses));
        }

        /// <summary>
        /// Create a whitelist address for an account
        /// </summary>
        /// <param name="model">The the address for whitelisting</param>
        /// <returns>The created whitelist address</returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<WhitelistAddressDto>> CreateWhitelistAddressAsync([FromBody] CreateWhitelistAddressDto model)
        {
            // Try to add new whitelist address
            var address = await _whitelistAddressCreateService.CreateWhitelistAddressAsync(model.UserId, model.CryptoCurrencyId, model.Address);

            // Return
            return Ok(_mapper.Map<WhitelistAddressDto>(address));
        }

        /// <summary>
        /// Validate a whitelist address for an account (So it can be used)
        /// </summary>
        /// <param name="id">The address to update</param>
        /// <param name="model">The the address for whitelisting</param>
        /// <returns>The created whitelist address</returns>
        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult<WhitelistAddressDto>> ValidateWhitelistAddressAsync([FromRoute] int id, [FromBody] ValidateWhitelistAddressDto model)
        {
            // Try to validate whitelist address
            var address = await _whitelistAddressUpdateService.ValidateWhitelistAddressAsync(id, model.IsValid, model.FailedReason);

            // Return
            return Ok(_mapper.Map<WhitelistAddressDto>(address));
        }
    }
}