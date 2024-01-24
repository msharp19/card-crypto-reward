using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities.Audit;

namespace CryptoCreditCardRewards.Models.Entities
{
    public class User : BaseEntity
    {
        public int Id { get; private set; }
        public string Email { get; private set; }
        public bool Active { get; private set; }
        public string AccountNumber { get; private set; }
        public DateTime? CompletedKycDate { get; private set; }

        public List<Transaction>? Transactions { get; private set; }
        public List<WalletAddress>? WalletAddresses { get; private set; }
        public List<UserRewardSelection>? UserRewardSelections { get; private set; }
        public List<Instruction>? Instructions { get; private set; }
        public List<WhitelistAddress>? WhitelistAddresses { get; private set; }

        public User(bool active, string email, string accountNumber)
        {
            Email = email;
            Active = active;
            AccountNumber = accountNumber;
        }

        public void CompleteKyc()
        {
            CompletedKycDate = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            Active = false;
        }

        public void Activate()
        {
            Active = true;
        }
    }
}
