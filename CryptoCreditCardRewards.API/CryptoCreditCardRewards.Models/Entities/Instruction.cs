using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities.Audit;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Events;

namespace CryptoCreditCardRewards.Models.Entities
{
    public class Instruction : BaseEntity
    {
        public int Id { get; private set; }
        public bool Active { get; private set; }
        public InstructionType Type { get; private set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Amount { get; private set; }
        public int UserId { get; private set; }
        public int? WalletAddressId { get; private set; }
        public DateTime? PickedUpDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }
        public int? ParentInstructionId { get; private set; }
        public int? WhitelistAddressId { get; private set; }
        public DateTime? FailedDate { get; private set; }
        public string? FailedReason { get; private set; }
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        [Column(TypeName = "decimal(18,8)")]
        public decimal ConversionRate { get; private set; }
        [Column(TypeName = "decimal(18,8)")]
        public decimal MonetaryFee { get; set; }
        [Column(TypeName = "decimal(18,8)")]
        public decimal MakeTransactionFee { get; set; }


        public User User { get; private set; }
        public WalletAddress? WalletAddress { get; private set; }
        public WhitelistAddress? WhitelistAddress { get; private set; }
        public Instruction? ParentInstruction { get; private set; }
        public List<Transaction> Transactions { get; private set; }

        public Instruction(bool active, decimal amount, int userId, int? walletAddressId, int? parentInstructionId, int? whitelistAddressId,
            DateTime fromDate, DateTime toDate, decimal conversionRate, decimal monetaryFee, decimal makeTransactionFee)
        {
            Active = active;
            Amount = amount;
            UserId = userId;
            WalletAddressId = walletAddressId;
            ConversionRate = conversionRate;
            ParentInstructionId = parentInstructionId;
            WhitelistAddressId = whitelistAddressId;
            FromDate = fromDate;
            MakeTransactionFee = makeTransactionFee;
            ToDate = toDate;
            MonetaryFee = monetaryFee;
        }

        public Instruction SetType(InstructionType type)
        {
            Type = type;

            // Fire event if the instruction is a monthly reward
            /*
            if (Type == InstructionType.MonthlyReward)
            {
                base.QueueDomainEvent(new CreatedRewardInstructionEvent(this));
            }
            */

            return this;
        }

        public void PickUp()
        {
            PickedUpDate = DateTime.UtcNow;
        }

        public void PutBack()
        {
            PickedUpDate = null;
        }

        public void Fail(string failedReason)
        {
            FailedDate = DateTime.UtcNow;
            FailedReason = failedReason;
        }

        public void SetComplete()
        {
            CompletedDate = DateTime.UtcNow;
        }
    }
}
