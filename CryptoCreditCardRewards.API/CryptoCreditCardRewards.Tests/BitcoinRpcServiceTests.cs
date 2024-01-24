using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Blockchain;
using CryptoCreditCardRewards.Services.Http;

namespace CryptoCreditCardRewards.Tests
{
    [TestClass]
    public class BitcoinRpcServiceTests
    {
        [TestMethod]
        public async Task GetTransactionByHashAsyncTest()
        {
            var service = new BitcoinRpcService("https://autumn-frequent-firefly.btc-testnet.discover.quiknode.pro/f3d14dd12e2be304ad6bbd410e7158bf8afa2c2c/", Models.Enums.NetworkType.Test);
            var hash = await service.GetTransactionByHashAsync("f3508dd83b9f11c9418c238f9248e4ff65250fe6aebd29c985330c67a16327a2");

            Assert.IsNotNull(hash);
        }

        [TestMethod]
        public async Task GetBalanceAsyncTest()
        {
            var service = new BitcoinRpcService("https://btc.getblock.io/f6264473-1688-4cac-b699-44a9796238ec/testnet/", Models.Enums.NetworkType.Test);
            var balance = await service.GetBalanceAsync("2NFfq7Rnz2zUFqanuGNEA7jdKgtF6yWRBDz");

            Assert.IsNotNull(balance);
        }
    }
}
