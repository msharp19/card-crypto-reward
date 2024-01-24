using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities.Audit;

namespace CryptoCreditCardRewards.Models.Entities
{
    public class WhitelistAddress : BaseEntity
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string Address { get; private set; }
        public DateTime? ProcessedDate { get; private set; }
        public bool Valid { get; private set; }
        public string? FailedReason { get; private set; }
        public int CryptoCurrencyId { get; private set; }

        public User User { get; private set; }
        public CryptoCurrency CryptoCurrency { get; private set; }
        public List<Instruction> Instructions { get; private set; }
        public List<Transaction> Transactions { get; private set; }

        public WhitelistAddress(int userId, int cryptoCurrencyId, string address)
        {
            UserId = userId;
            Address = address;
            CryptoCurrencyId = cryptoCurrencyId;
        }

        public void Process(bool valid, string? failedReason)
        {
            ProcessedDate = DateTime.Now;
            FailedReason = failedReason;
            Valid = valid;
        }
    }
}
