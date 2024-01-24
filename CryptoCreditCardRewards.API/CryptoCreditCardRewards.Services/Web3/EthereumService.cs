using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Accounts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Misc.Interfaces;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Utilities;
using Nethereum.Web3;
using CryptoCreditCardRewards.Models.Blockchain;
using System.Net;
using Newtonsoft.Json;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Utilities.Models;
using Nethereum.Util;
using CryptoCreditCardRewards.Models.Misc;

namespace CryptoCreditCardRewards.Services.Blockchain
{
    public class EthereumService : IBlockchainService
    {
        private readonly ICacheService _cacheService;
        private readonly EthereumSettings _ethereumSettings;
        private string _endpoint;

        public EthereumService(ICacheService cacheService, IOptions<EthereumSettings> ethereumSettings, string endpoint)
        {
            _cacheService = cacheService;
            _ethereumSettings = ethereumSettings.Value;
            _endpoint = endpoint;   
        }

        /// <summary>
        /// Make a transaction on the Ethereum blockchain
        /// </summary>
        /// <param name="endpoint">The endpoint to make the request on</param>
        /// <param name="fromAddress">The from address</param>
        /// <param name="pk">The from addresses PK</param>
        /// <param name="toAddress">The to address</param>
        /// <param name="amount">The amount to send (decimal)</param>
        /// <returns>The transaction hash</returns>
        public async Task<string> SendTransactionAsync(string fromAddress, string pk, string toAddress, decimal amount, decimal? fee)
        {
            var gasPrice = (fee ?? await GetGasPrice());
            var client = GetWeb3(pk, _endpoint);


            // Estimate
            var transferHandler = client.Eth.GetEtherTransferService();
            var gas = await transferHandler.EstimateGasAsync(toAddress, amount);

            var transactionInput = new TransactionInput()
            {
                From = fromAddress,
                Gas = new HexBigInteger(gas),
                To = toAddress,
                Value = new HexBigInteger(Web3.Convert.ToWei(amount)),
                GasPrice = new HexBigInteger(new BigInteger(1000000000) * new BigInteger(gasPrice)),
            };

            return await client.Eth.TransactionManager.SendTransactionAsync(transactionInput);
        }

        /// <summary>
        /// Get the esitmated transaction price 
        /// </summary>
        /// <param name="endpoint">The network endpoint</param>
        /// <param name="address">The address to send from</param>
        /// <param name="pk">The pk of the address to send from</param>
        /// <param name="amount">The amount to send</param>
        /// <returns>The esitmated transaction cost</returns>
        public async Task<TransactionFee> GetEstimatedTransactionPriceAsync(string address, string pk, decimal amount)
        {
            var gasPriceInGwei = await GetGasPrice();
            var client = GetWeb3(pk, _endpoint);

            var bal = await GetBalanceAsync(address);
            // Estimate
            var transferHandler = client.Eth.GetEtherTransferService();
            var gas = await transferHandler.EstimateGasAsync(address, amount);

            var transactionInput = new TransactionInput()
            {
                From = address,
                Gas = new HexBigInteger(gas),
                To = address,
                Value = new HexBigInteger(Web3.Convert.ToWei(amount)),
                GasPrice = new HexBigInteger(Web3.Convert.ToWei(gasPriceInGwei, UnitConversion.EthUnit.Gwei)),
            };

            // Convert
            return new TransactionFee()
            {
                MonetaryFee = Web3.Convert.FromWei(transactionInput.Gas.Value * transactionInput.GasPrice.Value),
                MakeTransactionFee = gasPriceInGwei
            };
        }

        /// <summary>
        /// Gets a transactions details
        /// </summary>
        /// <param name="hash">The transactions unique hash</param>
        /// <returns>A blockchain transactions details if exists</returns>
        public async Task<BlockchainTransaction?> GetTransactionDetailsAsync(string hash)
        {
            // Get client
            var client = new Web3(_endpoint, null, null);

            // Get transaction
            var transaction = await client.Eth.Transactions.GetTransactionByHash.SendRequestAsync(hash);
            if (transaction != null)
            {
                // If block number exists, then its completed
                if ((transaction?.BlockNumber?.Value ?? 0) <= 0)
                    return null;

                return new BlockchainTransaction()
                {
                    Amount = Web3.Convert.FromWei(transaction.Value.Value),
                    FromAddress = transaction.From,
                    ToAddress = transaction.To,
                    Fee = Web3.Convert.FromWei(transaction.Gas.Value * transaction.GasPrice.Value),
                    Hash = transaction.BlockHash
                };
            }

            // If here, state is pending
            return null;
        }

        /// <summary>
        /// Get an Ethereum addresses balance
        /// </summary>
        /// <param name="endpoint">A nodes endpoint</param>
        /// <param name="address">The address to get balance of</param>
        /// <returns>An addresses balance</returns>
        public async Task<decimal> GetBalanceAsync(string address)
        {
            // Get client
            var client = new Web3(_endpoint, null, null);

            // Get balance
            var balance = await client.Eth.GetBalance.SendRequestAsync(address);

            // Convert and return
            return Web3.Convert.FromWei(balance.Value);
        }

        /// <summary>
        /// Check the status of an on chain transaction
        /// </summary>
        /// <param name="hash">The hash to identify the transaction with</param>
        /// <returns>The current transaction state</returns>
        public async Task<TransactionState> GetTransactionStateAsync(string hash)
        {
            // Get client
            var client = new Web3(_endpoint, null, null);

            // Get transaction
            var transaction = await client.Eth.Transactions.GetTransactionByHash.SendRequestAsync(hash);
            if(transaction != null)
            {
                // If block number exists, then its completed
                if ((transaction?.BlockNumber?.Value ?? 0) > 0)
                    return TransactionState.Completed;
            }

            // If here, state is pending
            return TransactionState.Pending;
        }

        /// <summary>
        /// Check if an address is valid
        /// </summary>
        /// <returns>Returns true if an address is valid</returns>
        public bool IsAddressValid(string address)
        {
            return EthereumAddressUtility.ValidateAccountAddress(address);
        }

        /// <summary>
        /// Get a private key
        /// </summary>
        /// <param name="keyData"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string GetPrivateKey(string keyData, string password)
        {
            return EthereumAddressUtility.GetPrivateKey(password, keyData);
        }

        /// <summary>
        /// Checks if a network endpoint provided is valid
        /// </summary>
        /// <param name="networkEndpoint">The endpoint to check</param>
        /// <returns>True if the network is valid</returns>
        public async Task<bool> ValidateNetworkAsync(bool isTestNetwork)
        {
            try
            {
                // Generate an address on this network to get balance of
                var addressToTest = EthereumAddressUtility.GenerateAccount(_endpoint);

                // Get client
                var client = new Web3(_endpoint, null, null);

                // Get balance
                var balanceResponse = await client.Eth.GetBalance.SendRequestAsync(addressToTest.PublicKey);

                // Convert and return
                var balance = Web3.Convert.FromWei(balanceResponse.Value);

                // Assert its 0
                return (balance == 0m);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region Helpers

        /// <summary>
        /// Decrypts a pk and uses it to create a personal version of web3
        /// </summary>
        /// <param name="password">The encrypted PK</param>
        /// <param name="salt">The password salt</param>
        /// <param name="endpointAddress">The network endpoint address</param>
        /// <returns>A web3 object</returns>
        private Web3 GetWeb3(string pk, string endpointAddress)
        {
            IAccount accountPk = new Account(pk);
            return new Web3(accountPk, endpointAddress, null, null);
        }

        /// <summary>
        /// Get the current gas price to use
        /// </summary>
        /// <returns></returns>
        private async Task<decimal> GetGasPrice()
        {
            var cacheKey = $"gas_price";
            var cachedResult = _cacheService.GetValue<CachedResult<decimal>>(cacheKey);

            if ((cachedResult == null || cachedResult.DateCached.AddSeconds(10) <= DateTime.UtcNow))
            {
                var url = $"https://api.etherscan.io/api?module=gastracker&action=gasoracle&apikey={_ethereumSettings.EtherscanApiKey}";
                var gasStr = await new WebClient().DownloadStringTaskAsync(new Uri(url));
                var gas = JsonConvert.DeserializeObject<Gas>(gasStr);
                var gasPrice = (gas?.Result?.ProposeGasPrice ?? 10) + 5;

                _cacheService.SetValue<CachedResult<decimal>>(cacheKey, new CachedResult<decimal>() { Item = gasPrice, DateCached = DateTime.UtcNow }, TimeSpan.FromSeconds(10));
                return gasPrice;
            }

            return cachedResult.Item;
        }

        #endregion
    }
}
