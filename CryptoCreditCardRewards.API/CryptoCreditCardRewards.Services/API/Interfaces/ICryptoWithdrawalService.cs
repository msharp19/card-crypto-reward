using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface ICryptoWithdrawalService
    {
        /// <summary>
        /// Create a withdraw instruction
        /// </summary>
        /// <param name="walletAddressId">The wallet address to withdraw for</param>
        /// <param name="amount">The amount to withdraw</param>
        /// <returns>The instruction created</returns>
        Task<Instruction> WithdrawWalletBalanceAsync(int walletAddressId, int whitelistAddressIdTo, decimal amount);
    }
}
