using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Misc;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Http.Interfaces;

namespace CryptoCreditCardRewards.Services.Http
{
    public class CryptoCompareService : BaseHttpService, IConversionService
    {
        private readonly CryptoCompareSettings _cryptoCompareSettings;
        private readonly ILogger<CryptoCompareService> _logger;

        public CryptoCompareService(HttpClient client, ILogger<CryptoCompareService> logger, IOptions<CryptoCompareSettings> conversionOptions) : base(client)
        {
            _logger = logger;
            _cryptoCompareSettings = conversionOptions.Value;
        }

        /// <summary>
        /// Convert a currency from->to
        /// </summary>
        /// <param name="amount">The amount to convert</param>
        /// <param name="fromSymbol">The symbol from</param>
        /// <param name="toSymbol">The symbol to</param>
        /// <returns>The converted currency</returns>
        public async Task<CurrencyConversion> ConvertAsync(decimal amount, string fromSymbol, string toSymbol)
        {
            // Get the raw rate from crypto compare
            var rawRate = await GetAsync<dynamic>($"data/price?fsym={fromSymbol}&tsyms={toSymbol}", await GetAuthenticatedHeadersAsync());

            // Pull out the rate
            var jsonRawRate = JsonConvert.SerializeObject(rawRate);
            JsonObject jObejctRawRate = JsonObject.Parse(jsonRawRate);
            var rate = jObejctRawRate[toSymbol].GetValue<decimal>();

            // Conver the amount based on rate
            return new CurrencyConversion()
            {
                Value = amount * rate,
                Rate = rate,
            };
        }

        /// <summary>
        /// Checks if symbol is supported
        /// </summary>
        /// <param name="symbol">The symbol to check is supported</param>
        /// <returns>True if symbol is supported for implementation</returns>
        public async Task<bool> IsSupportedAsync(string? symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                return false;

            try
            {
                var rate = await ConvertAsync(1, "HKD", symbol);
                if (rate.Value != 0)
                    return true;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.StackTrace);
            }

            return false;
        }

        /// <summary>
        /// Gets the headers to make authenticated call 
        /// </summary>
        /// <returns>Http headers</returns>
        public override async Task<List<KeyValuePair<string, string>>> GetAuthenticatedHeadersAsync()
        {
            var token = await GetAccessTokenAsync();

            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Content-Type", "application/json"),
                new KeyValuePair<string, string>("ApiKey", _cryptoCompareSettings.ApiKey)
            };
        }
    }
}
