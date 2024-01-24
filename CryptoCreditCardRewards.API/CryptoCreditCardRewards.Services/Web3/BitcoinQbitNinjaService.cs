
using NBitcoin;
//using QBitNinja.Client;
//using QBitNinja.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    // Most likely not going to use this and use RPC instead
    /*
    public class BitcoinQbitNinjaService : IBlockchainService
    {
        private string _endpoint;
        private Network _network;

        public BitcoinQbitNinjaService(string endpoint, NetworkType network)
        {
            _endpoint = endpoint;
            _network = GetBitcoinNetworkFromType(network);
        }

        /// <summary>
        /// Get a bitcoin transaction by its hash
        /// </summary>
        /// <param name="transactionHash">The transaction hash</param>
        /// <returns>A Bitcoin transaction</returns>
        public async Task<GetTransactionResponse> GetTransactionByHashAsync(string transactionHash)
        {
            // Create a client
            var client = new QBitNinjaClient(_endpoint, _network);

            // Parse transaction id to NBitcoin.uint256 so the client can eat it
            var transactionId = uint256.Parse(transactionHash);

            // Query the transaction
            return client.GetTransaction(transactionId).Result;
        }

        /// <summary>
        /// Get the balance for a Bitcoin address
        /// </summary>
        /// <param name="address">The address to make request for</param>
        /// <returns>The balance for a Bitcoin address</returns>
        public async Task<decimal> GetBalanceAsync(string address)
        {
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
                if ((transaction?.Block?.BlockId ?? 0) > 0)
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
        /// <param name="minerFee">The fee for the miners</param>
        /// <param name="network">The network the transfer is being made on</param>
        /// <returns>A transaction hash</returns>
        /// <exception cref="Exception">Thrown if an error occours while processing the transfer</exception>
        public async Task<string> SendTransactionAsync(string fromAddress, string fromPrivateKey, string toAddress, decimal transferValue, decimal? fee)
        {
            var client = new QBitNinjaClient(_endpoint, _network);

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
            transaction = await BroadcastTransactionAsync(client, transaction);

            // Return the hash
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

                // Get unspent coins
                var unspentCoins = await GetUnspentCoinsAsync(_endpoint, _network, addressToTest.PublicKey);

                // Aggregate and return
                var balance = unspentCoins.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));

                // Assert its 0
                return (balance == 0m);
            }
            catch (Exception ex)
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
                if ((transaction?.Block?.BlockId ?? 0) <= 0)
                    return null;

                return new BlockchainTransaction()
                {
                    Amount = transaction.SpentCoins.Sum(x => ((Money)x.Amount).ToDecimal(MoneyUnit.BTC)),
                    FromAddress = transaction.SpentCoins.FirstOrDefault().TxOut.ScriptPubKey.GetDestinationAddress(_network)?.ToString(),
                    ToAddress = transaction.SpentCoins.FirstOrDefault().TxOut.ScriptPubKey.GetSignerAddress(_network)?.ToString(),
                    Fee = transaction.Fees.ToDecimal(MoneyUnit.BTC),
                    Hash = transaction.Block.BlockId.ToString()
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
        /// Broadcast the transaction into the Bitcoin network
        /// </summary>
        /// <param name="client">The client to broadcast on</param>
        /// <param name="transaction">The transaction to broadcast</param>
        /// <returns>The sent transaction</returns>
        /// <exception cref="Exception">Thrown if error sending bitcoin</exception>
        private async Task<Transaction> BroadcastTransactionAsync(QBitNinjaClient client, Transaction transaction)
        {
            // Broadcast the response
            BroadcastResponse broadcastResponse = await client.Broadcast(transaction);
            if (!broadcastResponse.Success)
                throw new Exception("Error broadcasting transaction " + broadcastResponse.Error.ErrorCode + " : " + broadcastResponse.Error.Reason);

            // Return the trransaction
            return transaction;
        }

        /// <summary>
        /// Get the unspent coins for a Bitcoin address
        /// </summary>
        /// <param name="network">The network to make request on</param>
        /// <param name="address">The address to make request for</param>
        /// <param name="endpoint">The endpoint to make request on</param>
        /// <returns>The balance for a Bitcoin address</returns>
        private async Task<List<Coin>> GetUnspentCoinsAsync(string endpoint, Network network, string address)
        {
            // Get the client to make requests to
            var client = new QBitNinjaClient(endpoint, network);

            // Build formatted address
            //var publicKeyAddress = new BitcoinWitPubKeyAddress(address, network);
            var publicKeyAddress = BitcoinAddress.Create(address, network);

            // Get and return balance
            var balance = await client.GetBalance(publicKeyAddress, true);

            // Tally up the coin operations
            var unspentCoins = new List<Coin>();
            foreach (var operation in balance.Operations)
            {
                unspentCoins.AddRange(operation.ReceivedCoins.Select(coin => coin as Coin));
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
    */
}
