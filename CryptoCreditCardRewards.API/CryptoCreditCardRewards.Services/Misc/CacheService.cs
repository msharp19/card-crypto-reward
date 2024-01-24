using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Services.Misc.Interfaces;

namespace CryptoCreditCardRewards.Services.Misc
{
    /// <summary>
    /// In house memory cache service (application cache)
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        /// <summary>
        /// The cancellation source token
        /// </summary>
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();

        /// <summary>
        /// The memory cache
        /// </summary>
        private readonly IMemoryCache _cache;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="cache"></param>
        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Sets an item of type T into the cache for key
        /// </summary>
        /// <typeparam name="T">Type to set</typeparam>
        /// <param name="key">The key to get the value with</param>
        /// <param name="value">The value to store in cahce</param>
        /// <param name="expiry">When the item should expire from cache</param>
        public void SetValue<T>(string key, T value, TimeSpan expiry)
        {
            var options = new MemoryCacheEntryOptions();
            options.SetAbsoluteExpiration(expiry);
            options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));
            _cache.Set(key, value, options);
        }

        /// <summary>
        /// Removes an item of type T from the cache for key
        /// </summary>
        /// <param name="key">The key to get the value with</param>
        public void RemoveValue(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// Removes an item of type T from the cache for key
        /// </summary>
        /// <param name="prefix">The key to get the value with</param>
        public void RemoveAllWithPrefixValue(string prefix)
        {
            var keys = GetAllKeys();
            foreach (var key in keys)
            {
                if (key.StartsWith(prefix))
                    _cache.Remove(key);
            }
        }

        /// <summary>
        /// Get all keys from memory cache
        /// </summary>
        /// <returns>All keys from memory cache</returns>
        private List<string> GetAllKeys()
        {
            // We want to get list from a private property so we have to use reflection
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);

            // Get the property as a list
            var collection = field.GetValue(_cache) as ICollection;

            //Create the return items
            var items = new List<string>();

            // Check to see if we have any keys to return
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    // Use reflection to key key value from object
                    var methodInfo = item.GetType().GetProperty("Key");
                    var val = methodInfo.GetValue(item);
                    // Add to our return list
                    items.Add(val.ToString());
                }
            }

            return items;
        }


        /// <summary>
        /// Resets the cache
        /// </summary>
        public void Reset()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }

            _resetCacheToken = new CancellationTokenSource();
        }

        public T GetValue<T>(string key)
        {
            if (_cache.TryGetValue(key, out T value))
                return value;

            return default(T);
        }
    }
}
