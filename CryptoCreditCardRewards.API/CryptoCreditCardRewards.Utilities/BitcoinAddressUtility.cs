using NBitcoin;
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
    public static class BitcoinAddressUtility
    {
        /// <summary>
        /// Generate a public and private key for the Bitcoin network
        /// </summary>
        /// <returns>A new crypto account (public/private key pair)</returns>
        public static KeyData GenerateAccount(string password, bool isTestNetwork)
        {
            var network = isTestNetwork ? Network.TestNet : Network.Main;

            // Create address/key
            var privateKey = new Key(fCompressedIn: true);
            var bitcoinPrivateKey = privateKey.GetWif(network);
            var bitcoinPublicKey = bitcoinPrivateKey.PubKey.GetAddress(ScriptPubKeyType.Segwit, network);
            var salt = Guid.NewGuid().ToString();

            var strPrivateKey = bitcoinPrivateKey.PrivateKey.ToHex();
            var strPublicKey = bitcoinPublicKey.ToString();

            // Encrypt
            var cipherText = EncryptionUtility.Encrypt(strPrivateKey, password, salt);

            // Convert to json
            var keyCipher = new KeyCipher()
            {
                Id = Guid.NewGuid().ToString(),
                Address = strPublicKey,
                Crypto = new Crypto()
                {
                    //Cipher = cipher,
                    Ciphertext = cipherText,
                    Kdfparams = new KdfParams()
                    {
                        Salt = salt,
                    }
                },
                Version = 1,
            };
            var json = JsonConvert.SerializeObject(keyCipher);

            // Return
            return new KeyData(strPublicKey, json);
        }

        /// <summary>
        /// Get a private key from encrpyted
        /// </summary>
        /// <param name="password">The password used to decrypt</param>
        /// <param name="keyData">The cypher text</param>
        /// <returns>A PK</returns>
        public static string GetPrivateKey(string password, string keyData)
        {
            // Deserialize
            var keyCipher = JsonConvert.DeserializeObject<KeyCipher>(keyData);

            // Decrypt cipher text
            var decrypted = EncryptionUtility.Decrypt(keyCipher.Crypto.Ciphertext, password, keyCipher.Crypto.Kdfparams.Salt);

            return decrypted;
        }

        /// <summary>
        /// Check if an bitcoin address provided is valid
        /// </summary>
        /// <param name="address">The address to check is valid</param>
        /// <returns>True if address is a valid bitcoin address</returns>
        public static bool ValidateAccountAddress(string address)
        {
            return new Regex("^[13][a-km-zA-HJ-NP-Z1-9]{25,34}$").IsMatch(address);
        }
    }
}
