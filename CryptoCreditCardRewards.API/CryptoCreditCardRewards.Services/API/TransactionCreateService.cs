using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Services.API.Interfaces;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Entity.Interfaces;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Blockchain;

namespace CryptoCreditCardRewards.Services.API
{
    public class TransactionCreateService : ITransactionCreateService
    {
        private readonly ITransactionService _transactionService;
        private readonly IWalletAddressService _walletAddressService;
        private readonly IBlockchainProviderFactory _blockchainServiceProviderFactory;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly IInstructionService _instructionService;
        private readonly IWhitelistAddressService _whitelistAddressService;
        private readonly IUserService _userService;

        public TransactionCreateService(ITransactionService transactionService, IWalletAddressService walletAddressService, IBlockchainProviderFactory blockchainServiceProviderFactory,
            ICryptoCurrencyService cryptoCurrencyService, IInstructionService instructionService, IWhitelistAddressService whitelistAddressService, IUserService userService)
        {
            _transactionService = transactionService;
            _walletAddressService = walletAddressService;
            _cryptoCurrencyService = cryptoCurrencyService;
            _instructionService = instructionService;
            _blockchainServiceProviderFactory = blockchainServiceProviderFactory;
            _userService = userService;
            _whitelistAddressService = whitelistAddressService;
        }

        /// <summary>
        /// Map a deposit transaction by its block chain hash
        /// </summary>
        /// <param name="walletAddressId">The wallet address to map the transaction of</param>
        /// <param name="hash">The hash to map from</param>
        /// <returns>A mapped transaction if valid</returns>
        public async Task<Transaction> CreateDepositTransactionAsync(int walletAddressId, string hash)
        {
            // Get wallet address
            var walletAddress = _walletAddressService.GetWalletAddressById(walletAddressId);
            if (walletAddress == null)
                throw new NotFoundException(FailedReason.WalletAddressDoesntExist, Property.WalletAddressId);

            // Check wallet is active
            if (!walletAddress.Active)
                throw new ForbidException(FailedReason.CryptoCurrencyIsCurrentlyInactive);

            // Check user is active
            var user = _userService.GetUser(walletAddress.UserId);
            if (user == null)
                throw new NotFoundException(FailedReason.UserDoesntExist);

            // Check user has passed kyc
            if (user.CompletedKycDate == null)
                throw new NotFoundException(FailedReason.KycIncomplete);

            // Get the crypto currency 
            var cryptoCurrency = _cryptoCurrencyService.GetCryptoCurrency(walletAddress.CryptoCurrencyId);
            if (cryptoCurrency == null)
                throw new NotFoundException(FailedReason.CryptoCurrencyDoesntExist);

            // Check crypto currency is active
            if (!cryptoCurrency.Active)
                throw new ForbidException(FailedReason.CryptoCurrencyIsCurrentlyInactive);

            // Get the blockchain service
            var blockChainService = _blockchainServiceProviderFactory.GetBlockchainService(cryptoCurrency.InfrastructureType, cryptoCurrency.IsTestNetwork ? NetworkType.Test : NetworkType.Main,
                cryptoCurrency.NetworkEndpoint);

            // Get the transaction from the blockchain
            var transaction = await blockChainService.GetTransactionDetailsAsync(hash);

            // Check there is a transaction
            if (transaction == null)
                throw new NotFoundException(FailedReason.TrasactionDoesntExist, Property.Hash);

            // Check the transaction was for this address
            if (transaction.ToAddress.ToLower().Trim() != walletAddress.Address.ToLower().Trim())
                throw new ForbidException(FailedReason.TransactionIsNotForThisWalletAddress, Property.WalletAddressId);

            // Check the transactions value
            if (transaction.Amount <= 0)
                throw new UnprocessableEntityException(FailedReason.TrasactionDoesntExistOrIsFor0Amount, Property.Hash);

            // Check for previously imported transactions with same hash
            var existingTransaction = _transactionService.GetTransactionByHash(hash);
            if (existingTransaction != null)
                throw new UnprocessableEntityException(FailedReason.TransactionAlreadyImported, Property.Hash);

            // Check wallet is whitelisted
            var whitelistAddress = _whitelistAddressService.GetWhitelistAddress(walletAddress.UserId, transaction.FromAddress, WhitelistAddressState.Vaild);
            if (whitelistAddress == null)
                throw new ForbidException(FailedReason.AddressNotWhitelisted, Property.Hash);
            
            // Create the transaction
            return await _transactionService.CreateTransactionAsync(TransactionType.Deposit, null, cryptoCurrency.Id, walletAddressId, null, whitelistAddress.Id, transaction.FromAddress, walletAddress.Address, 
               walletAddress.UserId, hash, transaction.Amount, false);
        }
    }
}
