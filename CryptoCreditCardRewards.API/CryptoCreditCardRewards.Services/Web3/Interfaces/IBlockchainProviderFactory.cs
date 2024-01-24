using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Services.Blockchain.Interfaces;

namespace CryptoCreditCardRewards.Services.Blockchain.Interfaces
{
    public interface IBlockchainProviderFactory
    {
        /// <summary>
        /// Get a block chain service based on its infrastructure type
        /// </summary>
        /// <param name="infrastructureType">The infrastructure type (network implementation)</param>
        /// <param name="networkType">The network type (Test/Main)</param>
        /// <param name="endpoint">A node endpoint to talk to blockchain with</param>
        /// <returns>A blockchain service for implementation</returns>
        /// <exception cref="NotFoundException">Thrown if infrastructure type is not supported</exception>
        IBlockchainService GetBlockchainService(InfrastructureType infrastructureType, NetworkType networkType, string endpoint);
    }
}
