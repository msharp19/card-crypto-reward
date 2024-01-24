using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Services.Misc.Interfaces
{
    public interface ICacheService
    {
        /// <summary>
        /// Sets an item of type T into the cache for key
        /// </summary>
        /// <typeparam name="T">Type to set</typeparam>
        /// <param name="key">The key to get the value with</param>
        /// <param name="value">The value to store in cahce</param>
        /// <param name="expiry">When the item should expire from cache</param>
        void SetValue<T>(string key, T value, TimeSpan expiry);

        /// <summary>
        /// Removes an item of type T from the cache for key
        /// </summary>
        /// <param name="key">The key to get the value with</param>
        void RemoveValue(string key);

        /// <summary>
        /// Removes an item of type T from the cache for key
        /// </summary>
        /// <param name="prefix">The key to get the value with</param>
        void RemoveAllWithPrefixValue(string prefix);

        /// <summary>
        /// Gets a value T from the cache
        /// </summary>
        /// <typeparam name="T">Value type held by cache</typeparam>
        /// <param name="key">The key of cache item</param>
        /// <returns>Item of type T</returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Resets the cache
        /// </summary>
        void Reset();
    }
}
