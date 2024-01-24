using Nethereum.KeyStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities.Audit;
using CryptoCreditCardRewards.Models.Events;
using CryptoCreditCardRewards.Utilities;

namespace CryptoCreditCardRewards.Models.Entities
{
    public class WalletAddress : BaseEntity
    {
        public int Id { get; private set; }
        public bool Active { get; private set; }
        public string Address { get; private set; }
        public string KeyData { get; private set; }
        public int CryptoCurrencyId { get; private set; }
        public int UserId { get; private set; }

        public CryptoCurrency CryptoCurrency { get; private set; }
        public User User { get; private set; }
        public List<UserAction> UserActions { get; private set; }
        public List<Instruction> Instructions { get; private set; }
        public List<Transaction> Transactions { get; private set; }

        public WalletAddress(bool active, string address, string keyData, int cryptoCurrencyId, int userId)
        {
            Active = active;
            Address = address;
            CryptoCurrencyId = cryptoCurrencyId;
            UserId = userId;
            KeyData = keyData;

            // Add the creation event
            base.QueueDomainEvent(new CreatedWalletAddressEvent(UserId, CryptoCurrencyId));
        }
    }
}
