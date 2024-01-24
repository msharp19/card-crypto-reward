using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Blockchain;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Misc;
using CryptoCreditCardRewards.Utilities.Models;

namespace CryptoCreditCardRewards.Services.Blockchain.Interfaces
{
    public interface IBlockchainService
    {
        /// <summary>
        /// Make a transaction on the blockchain
        /// </summary>
        /// <param name="fromAddress">The from address</param>
        /// <param name="fromPrivateKey">The from addresses PK</param>
        /// <param name="toAddress">The to address</param>
        /// <param name="amount">The amount to send (decimal)</param>
        /// <returns>The transaction hash</returns>
        Task<string> SendTransactionAsync(string fromAddress, string fromPrivateKey, string toAddress, decimal amount, decimal? fee);

        /// <summary>
        /// Get an addresses balance
        /// </summary>
        /// <param name="address">The address to get balance of</param>
        /// <returns>An addresses balance</returns>
        Task<decimal> GetBalanceAsync(string address);

        /// <summary>
        /// Check the status of an on chain transaction
        /// </summary>
        /// <param name="hash">The hash to identify the transaction with</param>
        /// <returns>The current transaction state</returns>
        Task<TransactionState> GetTransactionStateAsync(string hash);

        /// <summary>
        /// Checks if a network endpoint provided is valid
        /// </summary>
        /// <param name="networkEndpoint">The endpoint to check</param>
        /// <returns>True if the network is valid</returns>
        Task<bool> ValidateNetworkAsync(bool isTestNetwork);

        /// <summary>
        /// Check if an address is valid
        /// </summary>
        /// <returns>Returns true if an address is valid</returns>
        bool IsAddressValid(string address);

        /// <summary>
        /// Get a private key
        /// </summary>
        /// <param name="keyData"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        string GetPrivateKey(string keyData, string password);

        /// <summary>
        /// Gets a transactions details
        /// </summary>
        /// <param name="hash">The transactions unique hash</param>
        /// <returns>A blockchain transactions details if exists</returns>
        Task<BlockchainTransaction?> GetTransactionDetailsAsync(string hash);

        /// <summary>
        /// Get the esitmated transaction price 
        /// </summary>
        /// <param name="address">The address to send from</param>
        /// <param name="pk">The pk of the address to send from</param>
        /// <param name="amount">The amount to send</param>
        /// <returns>The esitmated transaction cost</returns>
        Task<TransactionFee> GetEstimatedTransactionPriceAsync(string address, string pk, decimal amount);
    }
}
