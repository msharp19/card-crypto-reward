using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CryptoCreditCardRewards.API.Helpers;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos;
using CryptoCreditCardRewards.Models.Dtos.Staking;
using CryptoCreditCardRewards.Models.Dtos.SystemWallets;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.API.Controllers
{
    [ApiController]
    [Route("v1/system-wallets")]
    public class SystemWalletController : ControllerBase
    {
        private readonly ISystemWalletAddressCreateService _systemWalletAddressCreateService;
        private readonly ISystemWalletAddressService _systemWalletAddressService;
        private readonly IMapper _mapper;

        public SystemWalletController(ISystemWalletAddressService systemWalletAddressService, ISystemWalletAddressCreateService systemWalletAddressCreateService, IMapper mapper)
        {
            _systemWalletAddressService = systemWalletAddressService;
            _systemWalletAddressCreateService = systemWalletAddressCreateService;
            _mapper = mapper;
        }

        /// <summary>
        /// Create a system wallet address
        /// </summary>
        /// <param name="model">The wallet data</param>
        /// <returns>The created wallet</returns>
        [HttpPost]
        [Route("address")]
        public async Task<ActionResult<SystemWalletAddressDto>> CreateWalletAddressAsync([FromBody] CreateSystemWalletAddressDto model)
        {
            // Try to add new wallet address
            var newWalletAddress = await _systemWalletAddressCreateService.CreateSystemWalletAddressAsync(model.CryptoCurrencyId, model.AddressType);

            // Return
            return Ok(_mapper.Map<SystemWalletAddressDto>(newWalletAddress));
        }

        /// <summary>
        /// Gets system wallet addresses
        /// </summary>
        /// <param name="search">A search term</param>
        /// <param name="cryptoCurrencyId">The crypto currency</param>
        /// <param name="state">The state of the wallet</param>
        /// <param name="orderProperty">The property or order results by</param>
        /// <param name="order">The order of the results</param>
        /// <param name="pageNumber">The page to return</param>
        /// <param name="perPage">The number of results per page</param>
        /// <returns>System wallet addresses</returns>
        [HttpGet]
        [Route("addresses")]
        public async Task<ActionResult<PagedResultsDto<SystemWalletAddressDto>>> GetSystemWalletAddressesPagedAsync([FromQuery] string? search,
            [FromQuery] int? cryptoCurrencyId, [FromQuery] ActiveState? state, [FromQuery] string? orderProperty, [FromQuery] Order order = Order.Ascending,
            [FromQuery] uint pageNumber = 1, [FromQuery] uint perPage = 30)
        {
            // Validate the order filter
            var validatedFilter = OrderHelper.ResolveOrderProperty(orderProperty, _systemWalletAddressService.GetSortProperties, out orderProperty);

            // Get filtered data
            var wallets = _systemWalletAddressService.GetSystemWalletAddressesPaged(search, cryptoCurrencyId, state, new Page((int)pageNumber, (int)perPage), new SortOrder(orderProperty, order));

            // Map and return
            return Ok(_mapper.Map<PagedResultsDto<SystemWalletAddressDto>>(wallets));
        }

        /// <summary>
        /// Get a system wallet address
        /// </summary>
        /// <param name="id">The system wallet address to get</param>
        /// <param name="state">The active state of the wallet address</param>
        /// <returns>The system wallet address</returns>
        /// <exception cref="NotFoundException">Thrown if 
        ///       - Doesnt exist
        /// </exception>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<SystemWalletAddressDto>> GetSystemWalletAddressAsync([FromRoute] int id, [FromQuery] ActiveState state = ActiveState.Active)
        {
            // Get the system wallet address
            var systemWalletAddress = _systemWalletAddressService.GetSystemWalletAddressById(id, state);
            if (systemWalletAddress == null)
                throw new NotFoundException(FailedReason.WalletAddressDoesntExist, Property.Id);

            // Map and return
            return Ok(_mapper.Map<SystemWalletAddressDto>(systemWalletAddress));
        }
    }
}
