using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities.Audit;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Entities
{
    public class Transaction : BaseEntity
    {
        public int Id { get; private set; }
        public bool Active { get; private set; }
        public int CryptoCurrencyId { get; private set; }
        public TransactionType Type { get; private set; }
        public int UserId { get; private set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Amount { get; private set; }
        public string Hash { get; private set; }
        public TransactionState State { get; private set; } = TransactionState.Pending; // Default pending
        public DateTime? ConfirmedDate { get; private set; }
        public string? FromAddress { get; private set; }
        public string? ToAddress { get; private set; }
        public DateTime? ReviewedDate { get; private set; }
        public string? ReviewedNotes { get; private set; }
        public bool FailedReview { get; private set; }
        public int WalletAddressId { get; private set; }
        public int? InstructionId { get; private set; }
        public int? WhitelistAddressId { get; private set; }
        public int? SystemWalletAddressId { get; private set; }

        public WhitelistAddress? WhitelistAddress { get; private set; }
        public WalletAddress WalletAddress { get; private set; }
        public SystemWalletAddress? SystemWalletAddress { get; private set; }
        public CryptoCurrency CryptoCurrency { get; private set; }
        public User User { get; private set; }
        public List<UserAction> UserActions { get; private set; }
        public Instruction? Instruction { get; private set; }

        public Transaction(bool active, TransactionType type, int? instructionId, int cryptoCurrencyId, int walletAddressId, int? systemWalletAddressId, int? whitelistAddressId, string? fromAddress, string? toAddress,
            int userId, string hash, decimal amount)
        {
            Active = active;
            CryptoCurrencyId = cryptoCurrencyId;
            WhitelistAddressId = whitelistAddressId;
            UserId = userId;
            Amount = amount;
            FromAddress = fromAddress;
            InstructionId = instructionId;
            ToAddress = toAddress;
            Type = type;
            Hash = hash;
            WalletAddressId = walletAddressId;
            SystemWalletAddressId = systemWalletAddressId;
        }

        public void Confirm(DateTime confirmedDate, TransactionState transactionState)
        {
            ConfirmedDate = confirmedDate;
            State = transactionState;
        }

        public void Review(string notes, bool failedReview)
        {
            FailedReview = failedReview;
            ReviewedDate = DateTime.UtcNow;
            ReviewedNotes = notes;
        }
    }
}
