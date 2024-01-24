using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Http;

namespace CryptoCreditCardRewards.Tests
{
    [TestClass]
    public class CryptoCompareServiceTests
    {
        [TestMethod]
        [DataRow(3d)]
        public async Task CreateEthereumAddress(double amount)
        {
            var settings = new CryptoCompareSettings()
            {
                ApiKey = "f37e81a9aa6eec1b15a1ba9e5c0d077bdbe62173a96f1eeb56616ea9a0eba854"
            };

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://min-api.cryptocompare.com/");

            var cryptoCompareService = new CryptoCompareService(client, null, Options.Create<CryptoCompareSettings>(settings));

            var convertedAmount = await cryptoCompareService.ConvertAsync((decimal)amount, "ETH", "HKD");

            Assert.IsNotNull(convertedAmount);
        }
    }
}
