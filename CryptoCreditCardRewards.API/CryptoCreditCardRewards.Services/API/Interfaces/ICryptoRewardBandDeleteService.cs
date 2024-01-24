using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Services.API.Interfaces
{
    public interface ICryptoRewardBandDeleteService
    {
        /// <summary>
        /// Delete a reward band
        /// </summary>
        /// <param name="id">The reward band to delete</param>
        /// <returns>An async task</returns>
        Task DeleteRewardBandAsync(int id);
    }
}
