using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface IWhitelistAddressUpdateService
    {
        /// <summary>
        /// Validate a whitelist address (mark as valid/invalid)
        /// </summary>
        /// <param name="id">The whitelist address to update</param>
        /// <param name="isValid">If the address is valid</param>
        /// <param name="failedReason">A reason the validation failed (if any)</param>
        /// <returns>The updated whitelist address</returns>
        Task<WhitelistAddress> ValidateWhitelistAddressAsync(int id, bool isValid, string? failedReason);
    }
}
