using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface IStakingCreateService
    {
        /// <summary>
        /// Stake Ethereum
        /// </summary>
        /// <param name="walletAddressId">The wallet to stake ETH for</param>
        /// <param name="amount">The amount to stake</param>
        /// <returns>An instruction to stake</returns>
        Task<Instruction> StakeCurrencyAsync(int walletAddressId, decimal amount);
    }
}
