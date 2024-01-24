using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities.Audit;

namespace CryptoCreditCardRewards.Models.Entities
{
    public class UserRewardSelection : BaseEntity
    {
        public int Id { get; private set; }
        public int CryptoCurrencyId { get; private set; }
        public int UserId { get; private set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal ContributionPercentage { get; private set; }

        public CryptoCurrency CryptoCurrency { get; private set; }
        public User User { get; private set; }

        public UserRewardSelection(int cryptoCurrencyId, int userId, decimal contributionPercentage)
        {
            CryptoCurrencyId = cryptoCurrencyId;
            UserId = userId;
            ContributionPercentage = contributionPercentage;
        }

        public UserRewardSelection(int id, decimal contributionPercentage)
        {
            Id = id;    
            ContributionPercentage = contributionPercentage;
        }

        public void SetContributionPercentage(decimal contributionPercentage)
        {
            ContributionPercentage = contributionPercentage;
        }
    }
}
