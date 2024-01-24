using Nethereum.KeyStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities.Audit;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Events;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Utilities;

namespace CryptoCreditCardRewards.Models.Entities
{
    public class SystemWalletAddress : BaseEntity
    {
        public int Id { get; private set; }
        public bool Active { get; private set; }
        public string Address { get; private set; }
        public string KeyData { get; private set; }
        public int CryptoCurrencyId { get; private set; }
        public AddressType AddressType { get; private set; }

        public CryptoCurrency CryptoCurrency { get; private set; }
        public List<Instruction> Instructions { get; private set; }
        public List<Transaction> Transactions { get; private set; }

        public SystemWalletAddress(bool active, AddressType addressType, string address, string keyData, int cryptoCurrencyId)
        {
            Active = active;
            Address = address;
            CryptoCurrencyId = cryptoCurrencyId;
            AddressType = addressType;
            KeyData = keyData;
        }
    }
}
