using Nethereum.KeyStore;
using Nethereum.Signer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Utilities.Models;

namespace CryptoCreditCardRewards.Utilities
{
    public static class EthereumAddressUtility
    {
        /// <summary>
        /// Generate a public and private key for the Ethereum network
        /// </summary>
        /// <param name="password">The password to encrpyt output with</param>
        /// <returns>A new crypto account (public/private key pair)</returns>
        public static KeyData GenerateAccount(string password)
        {
            EthECKey key = EthECKey.GenerateKey();
            byte[] privateKey = key.GetPrivateKeyAsBytes();
            string address = key.GetPublicAddress();
            var keyStore = new KeyStoreScryptService();

            string json = keyStore.EncryptAndGenerateKeyStoreAsJson(
                password: password,
                privateKey: privateKey,
                addresss: address);

            return new KeyData(address, json);
        }

        /// <summary>
        /// Get a private key from encrpyted
        /// </summary>
        /// <param name="password">The password used to decrypt</param>
        /// <param name="keyData">The cypher text</param>
        /// <returns>A PK</returns>
        public static string GetPrivateKey(string password, string keyData)
        {
            // Get from keystore
            var service = new KeyStoreService();
            var key = new EthECKey(
                    service.DecryptKeyStoreFromJson(password, keyData),
                    true);

            // Deserialize and return
            return key.GetPrivateKey();
        }

        /// <summary>
        /// Sanitize a private key (remove 0x00 from start if exists)
        /// </summary>
        /// <param name="privateKey">The private key to sanitize</param>
        /// <returns>A sanitized private key</returns>
        public static string SanitizePrivateKey(string privateKey)
        {
            // Remove 0x00 if it exists
            if (privateKey.StartsWith("0x00") && privateKey.Length == 68)
                return privateKey.Substring(4, 64);

            return privateKey;
        }

        /// <summary>
        /// Check if an ethereum address provided is valid
        /// </summary>
        /// <param name="address">The address to check is valid</param>
        /// <returns>True if address is a valid Ethereum address</returns>
        public static bool ValidateAccountAddress(string address)
        {
            return new Regex("^0x[a-fA-F0-9]{40}$").IsMatch(address);
        }
    }
}
