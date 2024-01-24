using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Utilities.Models
{
    public class KeyCipher
    {
        [JsonProperty("crypto")]
        public Crypto Crypto { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }

    public class Cipherparams
    {
        [JsonProperty("iv")]
        public string Iv { get; set; }
    }

    public class Crypto
    {
        [JsonProperty("cipher")]
        public string Cipher { get; set; }

        [JsonProperty("ciphertext")]
        public string Ciphertext { get; set; }

        [JsonProperty("cipherparams")]
        public Cipherparams Cipherparams { get; set; }

        [JsonProperty("kdf")]
        public string Kdf { get; set; }

        [JsonProperty("mac")]
        public string Mac { get; set; }

        [JsonProperty("kdfparams")]
        public KdfParams Kdfparams { get; set; }
    }

    public class KdfParams
    {
        [JsonProperty("n")]
        public int N { get; set; }

        [JsonProperty("r")]
        public int R { get; set; }

        [JsonProperty("p")]
        public int P { get; set; }

        [JsonProperty("dklen")]
        public int Dklen { get; set; }

        [JsonProperty("salt")]
        public string Salt { get; set; }
    }
}
