using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Utilities
{
    public static class StringUtility
    {
        /// <summary>
        /// Gets a randon string of length n
        /// </summary>
        /// <param name="n">length of random string to be produced</param>
        /// <returns></returns>
        public static string RandomString(int n, string possibilities = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!&$_+=")
        {
            return new string(Enumerable.Repeat(possibilities, n)
              .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Lower case the first letter of a string provided
        /// </summary>
        /// <param name="text">The text to lower first letter of</param>
        /// <returns>The value with the first letter in lower case</returns>
        public static string LowercaseFirstLetter(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            if (Char.IsUpper(text[0]) == true)
            {
                text = text.Replace(text[0], char.ToLower(text[0]));
                return text;
            }

            return text;
        }
    }
}
