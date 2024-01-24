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

namespace CryptoCreditCardRewards.Services.API
{
    public class StakingCreateService : IStakingCreateService
    {
        private readonly IInstructionService _instructionService;
        private readonly IWalletAddressService _walletAddressService;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        private readonly ISystemWalletAddressService _systemWalletAddressService;
        private readonly IBlockchainProviderFactory _blockchainServiceProviderFactory;
        private readonly WalletAddressSettings _walletAddressSettings;

        public StakingCreateService(IInstructionService instructionService, IBlockchainProviderFactory blockchainServiceProviderFactory, 
            IWalletAddressService walletAddressService, ICryptoCurrencyService cryptoCurrencyService, ITransactionService transactionService,
            ISystemWalletAddressService systemWalletAddressService, IUserService userService, IOptions<WalletAddressSettings> walletAddressSettings) 
        {
            _instructionService = instructionService;
            _blockchainServiceProviderFactory = blockchainServiceProviderFactory;
            _walletAddressService = walletAddressService;
            _transactionService = transactionService;
            _cryptoCurrencyService = cryptoCurrencyService;
            _systemWalletAddressService = systemWalletAddressService;
            _userService = userService;
            _walletAddressSettings = walletAddressSettings.Value;
        }

        /// <summary>
        /// Stake Ethereum based currencies
        /// </summary>
        /// <param name="walletAddressId">The wallet to stake ETH for</param>
        /// <param name="amount">The amount to stake</param>
        /// <returns>An instruction to stake</returns>
        public async Task<Instruction> StakeCurrencyAsync(int walletAddressId, decimal amount)
        {
            // Ensure +ve amount
            if (amount <= 0)
                throw new BadRequestException(FailedReason.AmountMustBePossitive, Property.Amount);

            // Get wallet
            var wallet = _walletAddressService.GetWalletAddressById(walletAddressId);
            if (wallet == null)
                throw new NotFoundException(FailedReason.WalletAddressDoesntExist, Property.WalletAddressId);

            // Check wallet is active
            if (!wallet.Active)
                throw new ForbidException(FailedReason.CryptoCurrencyIsCurrentlyInactive);

            // Get crypto that wallets for
            var cryptoCurrency = _cryptoCurrencyService.GetCryptoCurrency(wallet.CryptoCurrencyId, ActiveState.Active);
            if (cryptoCurrency == null)
                throw new NotFoundException(FailedReason.CurrencyDoesntExist);

            // Check crypto currency is active
            if (!cryptoCurrency.Active)
                throw new ForbidException(FailedReason.CryptoCurrencyIsCurrentlyInactive);

            // Ensure wallet is ethereum based
            if (!cryptoCurrency.SupportsStaking)
                throw new ForbidException(FailedReason.CryptoCurrencyDoesntSupportStaking);

            // Checks user exists
            var user = _userService.GetUser(wallet.UserId, ActiveState.Active);
            if (user == null)
                throw new NotFoundException(FailedReason.UserDoesntExist);

            // Checks user passed KYC
            if (!user.CompletedKycDate.HasValue)
                throw new ForbidException(FailedReason.KycIncomplete);

            // Check theres a system staking wallet otherwise this will fail later
            var systemWallet = _systemWalletAddressService.GetSystemWalletAddress(cryptoCurrency.Id, AddressType.StakingDeposit, ActiveState.Active);
            if (systemWallet == null)
                throw new UnprocessableEntityException(FailedReason.NoSystemWalletExistsForStakingThisCurrency);

            // Get the blockchain service
            var blockChainService = _blockchainServiceProviderFactory.GetBlockchainService(cryptoCurrency.InfrastructureType, cryptoCurrency.IsTestNetwork ? NetworkType.Test : NetworkType.Main,
                cryptoCurrency.NetworkEndpoint);

            // Get fees
            var fees = await blockChainService.GetEstimatedTransactionPriceAsync(wallet.Address, blockChainService.GetPrivateKey(wallet.KeyData, _walletAddressSettings.Password), amount);
            if (fees.MonetaryFee <= 0)
                throw new UnprocessableEntityException(FailedReason.GasPriceCouldNotBeDetermined);

            // Validate amount > balance of that asset (other withdrawals/staking being taken into account)
            var walletBalance = _transactionService.GetBalance(walletAddressId);
            var balance = (walletBalance?.SpendableBalance ?? 0) + (walletBalance?.OutstandingInstructionBalance ?? 0);
            if (balance < (amount + fees.MonetaryFee))
                throw new UnprocessableEntityException(FailedReason.AmountExceedsBalancePlusGasPrice, Property.Amount);

            // Create instruction
            return await _instructionService.CreateStakingDepositInstructionAsync(wallet.UserId, wallet.Id, DateTime.UtcNow, amount, fees.MonetaryFee, fees.MakeTransactionFee);
        }
    }
}
