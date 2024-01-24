using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Blockchain
{
    public class BlockchainTransaction
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }   
        public string Hash { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
    }
}
