using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface ITransactionCreateService
    {
        /// <summary>
        /// Map a deposit transaction by its block chain hash
        /// </summary>
        /// <param name="walletAddressId">The wallet address to map the transaction of</param>
        /// <param name="hash">The hash to map from</param>
        /// <returns>A mapped transaction if valid</returns>
        Task<Transaction> CreateDepositTransactionAsync(int walletAddressId, string hash);
    }
}
