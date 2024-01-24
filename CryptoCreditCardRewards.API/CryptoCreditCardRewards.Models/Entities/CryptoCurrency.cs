using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Entities.Audit;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Entities
{
    public class CryptoCurrency : BaseEntity
    {
        public int Id { get; private set; }
        public bool Active { get; private set; }
        public string Name { get; private set; }
        public string NetworkEndpoint { get; private set; }
        public string Symbol { get; private set; }
        public ConversionServiceType ConversionServiceType { get; private set; }
        public bool IsTestNetwork { get; private set; }
        public string? Description { get; private set; }
        public bool SupportsStaking { get; private set; }
        public InfrastructureType InfrastructureType { get; private set; }

        public List<Transaction> Transactions { get; private set; }
        public List<WalletAddress> WalletAddresses { get; private set; }
        public List<SystemWalletAddress> SystemWalletAddresses { get; private set; }
        public List<UserRewardSelection> UserRewardSelections { get; private set; }
        public List<WhitelistAddress> WhitelistAddresses { get; private set; }

        public CryptoCurrency(bool active, string name, string networkEndpoint, string symbol, bool isTestNetwork, string? description, bool supportsStaking,
            InfrastructureType infrastructureType, ConversionServiceType conversionServiceType)
        {
            Active = active;
            Name = name;
            NetworkEndpoint = networkEndpoint;
            Symbol = symbol;
            IsTestNetwork = isTestNetwork;
            Description = description;
            InfrastructureType = infrastructureType;
            SupportsStaking = supportsStaking;
            ConversionServiceType = conversionServiceType;
        }

        public void SetActiveState(bool state)
        {
            Active = state;
        }
    }
}
