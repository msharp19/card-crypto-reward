using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Exceptions;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;
using CryptoCreditCardRewards.Services.Misc.Interfaces;

namespace CryptoCreditCardRewards.Services.Blockchain
{
    public class BlockchainProviderFactory : IBlockchainProviderFactory
    {
        private readonly ICacheService _cacheService;
        private readonly IOptions<EthereumSettings> _ethereumSettings;

        public BlockchainProviderFactory(ICacheService cacheService, IOptions<EthereumSettings> ethereumSettings)
        {
            _cacheService = cacheService;
            _ethereumSettings = ethereumSettings;
        }

        /// <summary>
        /// Get a block chain service based on its infrastructure type
        /// </summary>
        /// <param name="infrastructureType">The infrastructure type (network implementation)</param>
        /// <param name="networkType">The network type (Test/Main)</param>
        /// <param name="endpoint">A node endpoint to talk to blockchain with</param>
        /// <returns>A blockchain service for implementation</returns>
        /// <exception cref="NotFoundException">Thrown if infrastructure type is not supported</exception>
        public IBlockchainService GetBlockchainService(InfrastructureType infrastructureType, NetworkType networkType, string endpoint)
        {
            switch (infrastructureType)
            {
                case InfrastructureType.BitcoinRpc: return new BitcoinRpcService(endpoint, networkType);
                //case InfrastructureType.BitcoinQbitNinja: return new BitcoinQbitNinjaService(endpoint, networkType);
                case InfrastructureType.EthereumRpc: return new EthereumService(_cacheService, _ethereumSettings, endpoint);
                default: throw new NotSupportedException($"The infrastructure of type: {infrastructureType} is not currently supported.");
            }
        }
    }
}
