using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CryptoCreditCardRewards.API.Helpers;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos;
using CryptoCreditCardRewards.Models.Dtos.Accounts;
using CryptoCreditCardRewards.Models.Dtos.CryptoCurrencies;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.Api.Interfaces;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.API.Controllers
{
    [ApiController]
    [Route("v1/crypto-currencies")]
    public class CryptoCurrencyController : ControllerBase
    {
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly ICryptoCurrencyCreateService _createCryptoService;
        private readonly ICryptoCurrencyUpdateService _cryptoCurrencyUpdateService;
        private readonly IMapper _mapper;
        private readonly ILogger<CryptoCurrencyController> _logger;

        public CryptoCurrencyController(ICryptoCurrencyService cryptoCurrencyService, ICryptoCurrencyCreateService createCryptoService, ICryptoCurrencyUpdateService cryptoCurrencyUpdateService, 
            IMapper mapper, ILogger<CryptoCurrencyController> logger)
        {
            _cryptoCurrencyService = cryptoCurrencyService;
            _createCryptoService = createCryptoService;
            _cryptoCurrencyUpdateService = cryptoCurrencyUpdateService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Create a crypto currency
        /// </summary>
        /// <param name="model">The account to create</param>
        /// <remarks>
        /// 
        /// </remarks>
        /// <returns>The created crypto currency</returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<CryptoCurrencyDto>> CreateCryptoCurrencyAsync([FromBody] CreateCryptoCurrencyDto model)
        {
            // Try to create new crypto currency
            var newCryptoCurrency = await _createCryptoService.CreateCryptoCurrencyAsync(model.Name, model.Symbol, model.Description, model.Active, model.NetworkEndpoint, model.IsTestNetwork,
                model.SupportsStaking, model.InfrastructureType, model.ConversionServiceType);

            return Ok(_mapper.Map<CryptoCurrencyDto>(newCryptoCurrency));
        }

        /// <summary>
        /// Deactivate a crypto currency
        /// </summary>
        /// <param name="id">The crypto currency to deactivate</param>
        /// <remarks>
        /// 
        /// </remarks>
        /// <returns>Ok</returns>
        [HttpPatch]
        [Route("{id}/deactivate")]
        public async Task<ActionResult> DeactivateCryptoCurrencyAsync([FromRoute] int id)
        {
            // Deactivate
            await _cryptoCurrencyUpdateService.DeactivateCryptoCurrencyAsync(id);

            return Ok();
        }

        /// <summary>
        /// Reactivate a crypto currency
        /// </summary>
        /// <param name="id">The crypto currency to reactivate</param>
        /// <remarks>
        /// 
        /// </remarks>
        /// <returns>The activated crypto currency</returns>
        [HttpPatch]
        [Route("{id}/activate")]
        public async Task<ActionResult<CryptoCurrencyDto>> ReactivateCryptoCurrencyAsync([FromRoute] int id)
        {
            // Deactivate
            var updatedUser = await _cryptoCurrencyUpdateService.ReactivateCryptoCurrencyAsync(id);

            return Ok(_mapper.Map<CryptoCurrencyDto>(updatedUser));
        }

        /// <summary>
        /// Get a page of crypto currencies
        /// </summary>
        /// <param name="search">Searches name of currency + symbol</param>
        /// <param name="symbol">The symbol of the currency</param>
        /// <param name="stakable">If the currency is stakable or not</param>
        /// <param name="state">The state of the currency</param>
        /// <returns>A crypto currency if it exists</returns>
        /// <exception cref="NotFoundException">Thrown if
        ///     - Currency cant be found 
        /// </exception>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<CryptoCurrencyDto>> GetCurrenciesAsync([FromQuery] string? symbol, [FromQuery] string? search, [FromQuery] bool? stakable,
            [FromQuery] string? orderProperty, [FromQuery] ActiveState state = ActiveState.Active, [FromQuery] Order order = Order.Ascending, 
            [FromQuery] uint pageNumber = 1, [FromQuery] uint perPage = 30)
        {
            // Validate the order filter
            var validatedFilter = OrderHelper.ResolveOrderProperty(orderProperty, _cryptoCurrencyService.GetSortProperties, out orderProperty);

            // Get filtered data
            var currencies = _cryptoCurrencyService.GetCurrenciesPaged(search, symbol, stakable, state, new Page((int)pageNumber, (int)perPage), new SortOrder(orderProperty, order));

            // Map and return
            return Ok(_mapper.Map<PagedResultsDto<CryptoCurrencyDto>>(currencies));
        }

        /// <summary>
        /// Get a crypto currency 
        /// </summary>
        /// <param name="id">The crypto currency to get</param>
        /// <param name="state">The active state of the currency</param>
        /// <returns>The crypto currency</returns>
        /// <exception cref="NotFoundException">Thrown if 
        ///       - Doesnt exist
        /// </exception>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CryptoCurrencyDto>> GetCurrencyAsync([FromRoute] int id, [FromQuery] ActiveState state = ActiveState.Active)
        {
            // Get the currency
            var currency = _cryptoCurrencyService.GetCryptoCurrency(id, state);
            if (currency == null)
                throw new NotFoundException(FailedReason.CurrencyDoesntExist, Property.Id);

            // Map and return
            return Ok(_mapper.Map<CryptoCurrencyDto>(currency));
        }
    }
}