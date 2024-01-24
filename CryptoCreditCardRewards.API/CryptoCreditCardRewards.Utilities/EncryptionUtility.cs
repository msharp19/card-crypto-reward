using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Utilities
{
    public class EncryptionUtility
    {
        /// <summary>
        /// Encrypts a string using 128bit AES encryption
        /// </summary>
        /// <param name="clearText">Text to encrypt</param>
        /// <param name="encryptionKey">Key to encrypt string with</param>
        /// <param name="salt">Random salt</param>
        /// <returns>Encrypted string</returns>
        public static string Encrypt(string clearText, string encryptionKey, string salt)
        {
            string EncryptionKey = encryptionKey;
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (System.Security.Cryptography.Aes encryptor = System.Security.Cryptography.Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, Encoding.ASCII.GetBytes(salt));
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        /// <summary>
        /// Decrypts a string using 128bit AES encryption
        /// </summary>
        /// <param name="cipherText">Text to decrypt</param>
        /// <param name="encryptionKey">Key to encrypt string with</param>
        /// <param name="salt">Random salt</param>
        /// <returns>Decrypted string</returns>
        public static string Decrypt(string cipherText, string encryptionKey, string salt)
        {
            string EncryptionKey = encryptionKey;
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (System.Security.Cryptography.Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, Encoding.ASCII.GetBytes(salt));
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

    }
}
