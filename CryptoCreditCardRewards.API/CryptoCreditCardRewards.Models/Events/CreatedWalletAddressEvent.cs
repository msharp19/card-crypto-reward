using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Events
{
    public class CreatedWalletAddressEvent : IDomainEvent
    {
        public int UserId { get; set; }
        public int CryptoCurrencyId { get; set; }

        public CreatedWalletAddressEvent(int userId, int cryptoCurrencyId)
        {
            UserId = userId;
            CryptoCurrencyId = cryptoCurrencyId;
        }
    }
}
