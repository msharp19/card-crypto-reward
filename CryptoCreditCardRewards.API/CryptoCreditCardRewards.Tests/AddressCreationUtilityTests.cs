using Microsoft.VisualStudio.TestTools.UnitTesting;
using CryptoCreditCardRewards.Utilities;

namespace CryptoCreditCardRewards.Tests
{
    [TestClass]
    public class AddressCreationUtilityTests
    {
        [TestMethod]
        [DataRow("ds57HJkfp5o35")]
        public void CreateEthereumAddress(string password)
        {
            var ethereumAddress = EthereumAddressUtility.GenerateAccount(password);

            Assert.IsNotNull(ethereumAddress);

            var decrpyted = EthereumAddressUtility.GetPrivateKey(password, ethereumAddress.PrivateData);

            Assert.IsNotNull(decrpyted);
        }

        [TestMethod]
        [DataRow("ds57HJkfp5o35")]
        public void CreateBitcoinAddress(string password)
        {
            var bitcoinAddress = BitcoinAddressUtility.GenerateAccount(password, true);

            Assert.IsNotNull(bitcoinAddress);

            var decrpyted = BitcoinAddressUtility.GetPrivateKey(password, bitcoinAddress.PrivateData);

            Assert.IsNotNull(decrpyted);
        }
    }
}