using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.Services.API
{
    public class WalletCreateService : IWalletCreateService
    {
        private readonly IWalletAddressService _walletAddressService;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly IUserService _userService;

        public WalletCreateService(IWalletAddressService walletAddressService, ICryptoCurrencyService cryptoCurrencyService,
            IUserService userService)
        {
            _walletAddressService = walletAddressService;
            _cryptoCurrencyService = cryptoCurrencyService;
            _userService = userService;
        }

        /// <summary>
        /// Add a wallet address for a user account
        /// </summary>
        /// <param name="userId">The user to add wallet address for</param>
        /// <param name="cryptoCurrencyId">The crypto currency to add to wallet</param>
        /// <returns>The new wallet address created</returns>
        public async Task<WalletAddress> AddWalletAddressToUserAsync(int userId, int cryptoCurrencyId)
        {
            // Check crypto currency is supported
            var currency = _cryptoCurrencyService.GetCryptoCurrency(cryptoCurrencyId);
            if (currency == null)
                throw new NotFoundException(FailedReason.CurrencyDoesntExist, Property.CryptoCurrencyId);

            // Check user exists
            var user = _userService.GetUser(userId);
            if(user == null)
                throw new NotFoundException(FailedReason.UserDoesntExist, Property.UserId);

            // Check user doesn't already have a wallet for this currency
            var existingWalletAddress = _walletAddressService.GetUsersCurrencyWalletAddress(userId, cryptoCurrencyId);
            if (existingWalletAddress != null)
                throw new UnprocessableEntityException(FailedReason.WalletAddressAlreadyExists, Property.UserId);

            // Create
            var walletAddress = await _walletAddressService.CreateWalletAddressAsync(userId, cryptoCurrencyId);

            // Return
            return walletAddress;
        }
    }
}
