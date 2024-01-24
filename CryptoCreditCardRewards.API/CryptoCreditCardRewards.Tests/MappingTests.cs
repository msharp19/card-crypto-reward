using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.API.Mapping;

namespace CryptoCreditCardRewards.Tests
{
    [TestClass]
    public class MappingTests
    {
        private readonly IMapper _mapper;

        public MappingTests() => _mapper = new MapperConfiguration(cfg => { cfg.AddProfile<CryptoCreditCardRewardsMappingProfile>(); }).CreateMapper();

        [TestMethod]
        public void MappingsSetupCorrectlyTest() => _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
