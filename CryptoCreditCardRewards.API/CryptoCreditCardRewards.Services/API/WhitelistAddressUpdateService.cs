using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.Services.API
{
    public class WhitelistAddressUpdateService : IWhitelistAddressUpdateService
    {
        private readonly IWhitelistAddressService _whitelistAddressService;
        private readonly IBlockchainProviderFactory _blockchainProviderFactory;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;

        public WhitelistAddressUpdateService(IWhitelistAddressService whitelistAddressService, ICryptoCurrencyService cryptoCurrencyService,
            IBlockchainProviderFactory blockchainProviderFactory)
        {
            _whitelistAddressService = whitelistAddressService;
            _blockchainProviderFactory = blockchainProviderFactory;
            _cryptoCurrencyService = cryptoCurrencyService;
        }

        /// <summary>
        /// Validate a whitelist address (mark as valid/invalid)
        /// </summary>
        /// <param name="id">The whitelist address to update</param>
        /// <param name="isValid">If the address is valid</param>
        /// <param name="failedReason">A reason the validation failed (if any)</param>
        /// <returns>The updated whitelist address</returns>
        public async Task<WhitelistAddress> ValidateWhitelistAddressAsync(int id, bool isValid, string? failedReason)
        {
            var whitelistAddress = _whitelistAddressService.GetWhitelistAddress(id, WhitelistAddressState.Any);
            if (whitelistAddress == null)
                throw new NotFoundException(FailedReason.WhitelistAddressDoesntExist, Property.Id);

            return await _whitelistAddressService.ValidateWhitelistAddressAsync(id, isValid, failedReason);
        }
    }
}
