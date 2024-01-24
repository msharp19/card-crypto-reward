using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Utilities;

namespace CryptoCreditCardRewards.Services.API
{
    public class CryptoWithdrawalService : ICryptoWithdrawalService
    {
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly IWalletAddressService _walletAddressService;
        private readonly IUserService _userService;
        private readonly IInstructionService _instructionService;
        private readonly ITransactionService _transactionService;
        private readonly IBlockchainProviderFactory _blockchainServiceProviderFactory;
        private readonly IWhitelistAddressService _whitelistAddressService; 
        private readonly WalletAddressSettings _walletAddressSettings;

        public CryptoWithdrawalService(ICryptoCurrencyService cryptoCurrencyService, IWalletAddressService walletAddressService, IUserService userService, IBlockchainProviderFactory blockchainServiceProviderFactory,
            IInstructionService instructionService, ITransactionService transactionService, IWhitelistAddressService whitelistAddressService, IOptions<WalletAddressSettings> walletAddressSettings)
        {
            _cryptoCurrencyService = cryptoCurrencyService;
            _walletAddressService = walletAddressService;
            _walletAddressSettings = walletAddressSettings.Value;
            _whitelistAddressService = whitelistAddressService;
            _blockchainServiceProviderFactory = blockchainServiceProviderFactory;
            _userService = userService;
            _instructionService = instructionService;
            _transactionService = transactionService;
        }

        /// <summary>
        /// Create a withdraw instruction
        /// </summary>
        /// <param name="walletAddressId">The wallet address to withdraw for</param>
        /// <param name="amount">The amount to withdraw</param>
        /// <returns>The instruction created</returns>
        public async Task<Instruction> WithdrawWalletBalanceAsync(int walletAddressId, int whitelistAddressIdTo, decimal amount)
        {
            // Get wallet
            var wallet = _walletAddressService.GetWalletAddressById(walletAddressId);
            if (wallet == null)
                throw new NotFoundException(FailedReason.WalletAddressDoesntExist, Property.WalletAddressId);

            // Check user is active
            var user = _userService.GetUser(wallet.UserId);
            if (user == null)
                throw new NotFoundException(FailedReason.UserDoesntExist);

            // Check user has passed kyc
            if (user.CompletedKycDate == null)
                throw new NotFoundException(FailedReason.KycIncomplete);

            // Check wallet is active
            if (!wallet.Active)
                throw new ForbidException(FailedReason.CryptoCurrencyIsCurrentlyInactive);

            // Get the crypto
            var cryptoCurrency = _cryptoCurrencyService.GetCryptoCurrency(wallet.CryptoCurrencyId);
            if (cryptoCurrency == null)
                throw new UnprocessableEntityException(FailedReason.CryptoCurrencyIsCurrentlyInactive);

            // Check crypto currency is active
            if (!cryptoCurrency.Active)
                throw new ForbidException(FailedReason.CryptoCurrencyIsCurrentlyInactive);

            // Get the blockchain service
            var blockChainService = _blockchainServiceProviderFactory.GetBlockchainService(cryptoCurrency.InfrastructureType, cryptoCurrency.IsTestNetwork ? NetworkType.Test : NetworkType.Main,
                cryptoCurrency.NetworkEndpoint);

            // Get fees
            var fees = await blockChainService.GetEstimatedTransactionPriceAsync(wallet.Address, blockChainService.GetPrivateKey(wallet.KeyData, _walletAddressSettings.Password), amount);
            if (fees.MonetaryFee <= 0)
                throw new UnprocessableEntityException(FailedReason.GasPriceCouldNotBeDetermined);

            // Get whitelist address
            var whitelistAddress = _whitelistAddressService.GetWhitelistAddress(whitelistAddressIdTo, WhitelistAddressState.Vaild);
            if (whitelistAddress == null)
                throw new NotFoundException(FailedReason.WhitelistAddressDoesntExist, Property.WhitelistAddressIdTo);

            // Check the whitelist address is valid
            if (!whitelistAddress.Valid)
                throw new UnprocessableEntityException(FailedReason.WhitelistAddressNotValid, Property.WhitelistAddressIdTo);

            // Validate amount > balance of that asset (other withdrawals/staking being taken into account)
            var balance = _transactionService.GetBalance(walletAddressId)?.SpendableBalance ?? 0;
            if (balance < (amount + fees.MonetaryFee))
                throw new UnprocessableEntityException(FailedReason.AmountExceedsBalancePlusGasPrice, Property.Amount);

            // Create instruction
            return await _instructionService.CreateWithdrawalInstructionAsync(wallet.UserId, walletAddressId, whitelistAddress.Id, DateTime.UtcNow, amount, fees.MonetaryFee, fees.MakeTransactionFee);
        }
    }
}
