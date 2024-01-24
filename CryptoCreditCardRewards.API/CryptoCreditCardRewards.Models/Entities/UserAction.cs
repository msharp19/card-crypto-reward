using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities.Audit;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Entities
{
    public class UserAction : BaseEntity
    {
        public int Id { get; private set; }
        public UserActionType ActionType { get; private set; }
        public int? TransactionId { get; private set; }
        public int? WalletAddressId { get; private set; }

        public Transaction? Transaction { get; private set; }
        public WalletAddress? WalletAddress { get; private set; }

        public UserAction(UserActionType actionType, int? transactionId, int? walletAddressId)
        {
            ActionType = actionType;  
            TransactionId = transactionId;  
            WalletAddressId = walletAddressId;
        }
    }
}
