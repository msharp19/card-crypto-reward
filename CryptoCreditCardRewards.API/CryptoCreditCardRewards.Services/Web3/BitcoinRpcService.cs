
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.RPC;
//using QBitNinja.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Blockchain;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Misc;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Utilities;
using CryptoCreditCardRewards.Utilities.Models;

namespace CryptoCreditCardRewards.Services.Blockchain
{
    public class BitcoinRpcService : IBlockchainService
    {
        private string _endpoint;
        private Network _network;

        public BitcoinRpcService(string endpoint, NetworkType network)
        {
            _endpoint = endpoint;
            _network = GetBitcoinNetworkFromType(network);
        }

        /// <summary>
        /// Get a bitcoin transaction by its hash
        /// </summary>
        /// <param name="transactionHash">The transaction hash</param>
        /// <returns>A Bitcoin transaction</returns>
        public async Task<RawTransactionInfo> GetTransactionByHashAsync(string transactionHash)
        {
            RPCCredentialString credentials = RPCCredentialString.Parse(_endpoint);
            RPCClient client = new RPCClient(credentials, _endpoint, _network);

            // Parse transaction id to NBitcoin.uint256 so the client can eat it
            var transactionId = uint256.Parse(transactionHash);

            // Query the transaction
            return client.GetRawTransactionInfo(transactionId);
        }

        /// <summary>
        /// Get the balance for a Bitcoin address
        /// </summary>
        /// <param name="network">The network to make request on</param>
        /// <param name="address">The address to make request for</param>
        /// <param name="endpoint">The endpoint to make request on</param>
        /// <returns>The balance for a Bitcoin address</returns>
        public async Task<decimal> GetBalanceAsync(string address)
        {
            RPCCredentialString credentials = RPCCredentialString.Parse(_endpoint);
            RPCClient client = new RPCClient(credentials, _endpoint, _network);

            // Build formatted address
            //var key = BitcoinWitPubKeyAddress.Create(address, _network);
            //client.ImportAddress(key);

            // Get and return balance
            //return (await client.GetBalanceAsync())?.ToDecimal(MoneyUnit.BTC) ?? 0m;

            // Get unspent coins
            var unspentCoins = await GetUnspentCoinsAsync(_endpoint, _network, address);

            // Aggregate and return
            return unspentCoins.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));
        }

        /// <summary>
        /// Check the status of an on chain transaction
        /// </summary>
        /// <param name="hash">The hash to identify the transaction with</param>
        /// <returns>The current transaction state</returns>
        public async Task<TransactionState> GetTransactionStateAsync(string hash)
        {
            // Get transaction
            var transaction = await GetTransactionByHashAsync(hash);
            if (transaction != null)
            {
                // If block number exists, then its completed
                if ((transaction?.BlockHash ?? 0) > 0)
                    return TransactionState.Completed;
            }

            // If here, state is pending
            return TransactionState.Pending;
        }

        /// <summary>
        /// Transfer Bitcoin from one party to another
        /// </summary>
        /// <param name="fromPrivateKey">The private key for an address to send from</param>
        /// <param name="transferValue">The value to send</param>
        /// <param name="toAddress">The address to send the value to</param>
        /// <returns>A transaction hash</returns>
        /// <exception cref="Exception">Thrown if an error occours while processing the transfer</exception>
        public async Task<string> SendTransactionAsync(string fromAddress, string fromPrivateKey, string toAddress, decimal transferValue, decimal? fee)
        {
            var credentials = RPCCredentialString.Parse(_endpoint);
            var client = new RPCClient(credentials, _endpoint, _network);

            // Create a transaction
            var transaction = Transaction.Create(_network);

            //Load address
            var fromAddressPrivateKey = new BitcoinSecret(fromPrivateKey, _network);

            // Get public key from private key
            var fromAddressPublicKey = fromAddressPrivateKey.GetAddress(ScriptPubKeyType.Segwit);

            // Build formatted address
            var toAddressPublicKey = BitcoinAddress.Create(toAddress, _network);

            // Get all unspent coins for the sender (raw balance)
            var unspentCoinsOfSender = await GetUnspentCoinsAsync(_endpoint, _network, toAddress);

            // Get fee
            var minerFee = (fee.HasValue) ? fee.Value : (await GetEstimatedTransactionPriceAsync(fromAddressPublicKey.ScriptPubKey.ToString(), fromAddressPrivateKey.ToString(), transferValue)).MakeTransactionFee;

            // Build TX
            var txBuilder = _network.CreateTransactionBuilder();
            var tx = txBuilder
                .AddCoins(unspentCoinsOfSender)
                .AddKeys(fromAddressPrivateKey)
                .Send(toAddressPublicKey, new Money(transferValue, MoneyUnit.BTC))
                .SetChange(fromAddressPublicKey)
                .SendFees(Money.Coins(minerFee))
                .BuildTransaction(true);

            // Confirm verified
            var verified = txBuilder.Verify(tx);
            if (!verified)
                throw new Exception("BTC TX could not be verified");

            //Send transaction
            var hex = transaction.ToHex().ToString();

            // Broadcast transaction
            await client.SendRawTransactionAsync(tx);

            // Return hash
            return hex;
        }

        /// <summary>
        /// Check if an address is valid
        /// </summary>
        /// <returns>Returns true if an address is valid</returns>
        public bool IsAddressValid(string address)
        {
            return BitcoinAddressUtility.ValidateAccountAddress(address);
        }

        /// <summary>
        /// Get a private key
        /// </summary>
        /// <param name="keyData"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string GetPrivateKey(string keyData, string password)
        {
            return BitcoinAddressUtility.GetPrivateKey(password, keyData);
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
                var addressToTest = BitcoinAddressUtility.GenerateAccount(_endpoint, isTestNetwork);

                // Aggregate and return
                var balance = await GetBalanceAsync(addressToTest.PublicKey);

                // Assert its 0
                return (balance == 0m);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a transactions details
        /// </summary>
        /// <param name="hash">The transactions unique hash</param>
        /// <returns>A blockchain transactions details if exists</returns>
        public async Task<BlockchainTransaction?> GetTransactionDetailsAsync(string hash)
        {
            // Get transaction 
            var transaction = await GetTransactionByHashAsync(hash);
            if (transaction != null)
            {
                // If block number exists, then its completed
                if ((transaction?.BlockHash ?? 0) <= 0)
                    return null;

                return new BlockchainTransaction()
                {
                    Amount = transaction.Transaction.TotalOut.ToDecimal(MoneyUnit.BTC),
                    FromAddress = transaction.Transaction.Outputs.FirstOrDefault().ScriptPubKey.GetDestinationAddress(_network)?.ToString(),
                    ToAddress = transaction.Transaction.Outputs.FirstOrDefault().ScriptPubKey.GetSignerAddress(_network)?.ToString(),
                    Fee = 0, // TODO
                    Hash = transaction.BlockHash.ToString()
                };
            }

            // If here, state is pending
            return null;
        }

        /// <summary>
        /// Get the esitmated transaction price 
        /// </summary>
        /// <returns>The esitmated transaction cost</returns>
        public async Task<TransactionFee> GetEstimatedTransactionPriceAsync(string address, string pk, decimal amount)
        {
            return await Task.FromResult(new TransactionFee()
            {
                MonetaryFee = 0.0005m,
                MakeTransactionFee = 0.0005m
            });
        }

        #region Helpers

        /// <summary>
        /// Get the unspent coins for a Bitcoin address
        /// </summary>
        /// <param name="network">The network to make request on</param>
        /// <param name="address">The address to make request for</param>
        /// <param name="endpoint">The endpoint to make request on</param>
        /// <returns>The balance for a Bitcoin address</returns>
        private async Task<List<Coin>> GetUnspentCoinsAsync(string endpoint, Network network, string address)
        {
            var credentials = RPCCredentialString.Parse(_endpoint);
            var client = new RPCClient(credentials, _endpoint, _network);

            // Build formatted address
            var publicKeyAddress = BitcoinAddress.Create(address, network);

            // Get and return balance
            var unspentCoinsRaw = await client.ListUnspentAsync(new ListUnspentOptions(), publicKeyAddress);

            // Tally up the coin operations
            var unspentCoins = new List<Coin>();
            foreach (var operation in unspentCoinsRaw)
            {
                unspentCoins.Add(operation.AsCoin());
            }

            // Return
            return unspentCoins;
        }

        /// <summary>
        /// Turn bitcoin network enum to NBitcoin constant
        /// </summary>
        /// <param name="network">The bitcoin network enum</param>
        /// <returns>An NBitcoin constant</returns>
        /// <exception cref="NotImplementedException">Thrown if bitcoin network now supported</exception>
        private Network GetBitcoinNetworkFromType(NetworkType network)
        {
            switch (network)
            {
                case NetworkType.Test: return Network.TestNet;
                case NetworkType.Main: return Network.Main;
                default: throw new NotSupportedException($"Bitcoin Network {network} not supported.");
            }
        }

        #endregion
    }
}
