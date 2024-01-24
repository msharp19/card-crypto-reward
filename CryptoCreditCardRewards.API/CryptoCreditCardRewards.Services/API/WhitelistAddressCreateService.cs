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
    public class WhitelistAddressCreateService : IWhitelistAddressCreateService
    {
        private readonly IWhitelistAddressService _whitelistAddressService;
        private readonly IBlockchainProviderFactory _blockchainProviderFactory;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;

        public WhitelistAddressCreateService(IWhitelistAddressService whitelistAddressService, ICryptoCurrencyService cryptoCurrencyService,
            IBlockchainProviderFactory blockchainProviderFactory)
        {
            _whitelistAddressService = whitelistAddressService;
            _blockchainProviderFactory = blockchainProviderFactory;
            _cryptoCurrencyService = cryptoCurrencyService;
        }

        /// <summary>
        /// Add an address to whitelist
        /// </summary>
        /// <param name="userId">The user the address is for</param>
        /// <param name="cryptoCurrencyId">The crypto currency the address is for</param>
        /// <param name="address">The address</param>
        /// <returns>A new whitelist address</returns>
        public async Task<WhitelistAddress> CreateWhitelistAddressAsync(int userId, int cryptoCurrencyId, string address)
        {
            var cryptoCurrency = _cryptoCurrencyService.GetCryptoCurrency(cryptoCurrencyId);
            if (cryptoCurrency == null)
                throw new UnprocessableEntityException(FailedReason.CryptoCurrencyDoesntExist);

            var blockChainService = _blockchainProviderFactory.GetBlockchainService(cryptoCurrency.InfrastructureType, cryptoCurrency.IsTestNetwork ? NetworkType.Test : NetworkType.Main, 
                cryptoCurrency.NetworkEndpoint);

            // Check if address is valid
            var isAddressValid = blockChainService.IsAddressValid(address);
            if (!isAddressValid)
                throw new UnprocessableEntityException(FailedReason.AddressIsNotValid, Property.ToAddress);

            // Ensure is not already whitelisted for user
            var alreadyWhitelisted = _whitelistAddressService.IsAddressAlreadyWhitelisted(userId, address);
            if (alreadyWhitelisted)
                throw new UnprocessableEntityException(FailedReason.AddressAlreadyWhitelisted, Property.Address);

            // Create
            return await _whitelistAddressService.CreateWhitelistAddressAsync(userId, cryptoCurrencyId, address);
        }
    }
}
