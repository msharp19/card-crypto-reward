using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CryptoCreditCardRewards.API.Helpers;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos;
using CryptoCreditCardRewards.Models.Dtos.Instructions;
using CryptoCreditCardRewards.Models.Dtos.Staking;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.API.Controllers
{
    [ApiController]
    [Route("v1/instructions")]
    public class InstructionController : ControllerBase
    {
        private readonly IInstructionService _instructionService;
        private readonly IMapper _mapper;

        public InstructionController(IInstructionService instructionServicee, IMapper mapper)
        {
            _instructionService = instructionServicee;
            _mapper = mapper;
        }

        /// <summary>
        /// Get an instruction
        /// </summary>
        /// <param name="id">The instruction to get</param>
        /// <param name="state">The active state of the instruction</param>
        /// <returns>The instruction</returns>
        /// <exception cref="NotFoundException">Thrown if 
        ///       - Doesnt exist
        /// </exception>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<InstructionDto>> GetInstructionAsync([FromRoute] int id, [FromQuery] ActiveState state = ActiveState.Active)
        {
            // Get the instruction
            var instruction = _instructionService.GetInstruction(id, state);
            if (instruction == null)
                throw new NotFoundException(FailedReason.InstructionNotFound, Property.Id);

            // Map and return
            return Ok(_mapper.Map<InstructionDto>(instruction));
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
        /// <param name="order">The order to display results in</param>
        /// <param name="orderProperty">The property to sort by</param>
        /// <param name="pageNumber">The page number to return (starts from 0)</param>
        /// <param name="perPage">The total results to return per page</param>
        /// <returns>Paged instructions</returns>
        [HttpGet]
        [Route("user/{userId}")]
        public async Task<ActionResult<PagedResultsDto<InstructionDto>>> GetUsersInstructionsPagedAsync([FromRoute] int userId, [FromQuery] string? search, [FromQuery] InstructionType? type,
            [FromQuery] int? walletAddressId, [FromQuery] decimal? fromAmount, [FromQuery] decimal? toAmount, [FromQuery] int? parentInstructionId, [FromQuery] string? orderProperty, 
            [FromQuery] ActiveState state = ActiveState.Active, [FromQuery] Order order = Order.Ascending, [FromQuery] uint pageNumber = 1, [FromQuery] uint perPage = 30)
        {
            // Validate the order filter
            var validatedFilter = OrderHelper.ResolveOrderProperty(orderProperty, _instructionService.GetSortProperties, out orderProperty);

            // Get filtered data
            var users = _instructionService.GetInstructionsPaged(userId, search, type, walletAddressId, fromAmount, toAmount, parentInstructionId, 
                state, new Page((int)pageNumber, (int)perPage), new SortOrder(orderProperty, order));

            // Map and return
            return Ok(_mapper.Map<PagedResultsDto<InstructionDto>>(users));
        }
    }
}
