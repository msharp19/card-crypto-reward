using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Utilities.Models
{
    public class KeyData
    {
        public string PublicKey ;
        public string PrivateData ;

        public KeyData(string publicKey, string privateData)
        {
            PublicKey = publicKey;
            PrivateData = privateData;
        }
    }
}
