using AutoMapper;
using CryptoCreditCardRewards.Models;
using CryptoCreditCardRewards.Models.Dtos;
using CryptoCreditCardRewards.Models.Dtos.Accounts;
using CryptoCreditCardRewards.Models.Dtos.CryptoCurrencies;
using CryptoCreditCardRewards.Models.Dtos.CryptoRewardSpendBands;
using CryptoCreditCardRewards.Models.Dtos.Instructions;
using CryptoCreditCardRewards.Models.Dtos.Staking;
using CryptoCreditCardRewards.Models.Dtos.SystemWallets;
using CryptoCreditCardRewards.Models.Dtos.Transactions;
using CryptoCreditCardRewards.Models.Dtos.UserRewardSelections;
using CryptoCreditCardRewards.Models.Dtos.Wallet;
using CryptoCreditCardRewards.Models.Dtos.WalletAddresses;
using CryptoCreditCardRewards.Models.Dtos.WhitelistedAddresses;
using CryptoCreditCardRewards.Models.Entities;
using CryptoCreditCardRewards.Models.Misc;

namespace CryptoCreditCardRewards.API.Mapping
{
    public class CryptoCreditCardRewardsMappingProfile : Profile
    {
        public CryptoCreditCardRewardsMappingProfile()
        {
            #region Accounts

            CreateMap<User, UserDto>()
               .ReverseMap();

            CreateMap<PagedResults<User>, PagedResultsDto<UserDto>>()
               .ReverseMap();

            #endregion

            #region Crypto Currencies

            CreateMap<CryptoCurrency, CryptoCurrencyDto>()
               .ReverseMap();

            CreateMap<PagedResults<CryptoCurrency>, PagedResultsDto<CryptoCurrencyDto>>()
               .ReverseMap();

            #endregion

            #region Crypto Reward Spend Bands

            CreateMap<CryptoRewardSpendBand, CryptoRewardSpendBandDto>()
               .ReverseMap();

            CreateMap<PagedResults<CryptoRewardSpendBand>, PagedResultsDto<CryptoRewardSpendBandDto>>()
              .ReverseMap();

            #endregion

            #region System Wallet Addresses

            CreateMap<SystemWalletAddress, SystemWalletAddressDto>()
               .ReverseMap();

            CreateMap<PagedResults<SystemWalletAddress>, PagedResultsDto<SystemWalletAddressDto>>()
               .ReverseMap();

            #endregion


            #region Transactions

            CreateMap<Transaction, TransactionDto>()
               .ReverseMap();

            CreateMap<PagedResults<Transaction>, PagedResultsDto<TransactionDto>>()
               .ReverseMap();

            #endregion

            #region User Reward Selections

            CreateMap<UserRewardSelection, UserRewardSelectionDto>()
               .ReverseMap();

            CreateMap<UpdateRewardSelectionDto, UserRewardSelection>()
                .ForMember(dest => dest.UserId, src => src.Ignore())
                .ForMember(dest => dest.CryptoCurrencyId, src => src.Ignore());

            CreateMap<PagedResults<UserRewardSelection>, PagedResultsDto<UserRewardSelectionDto>>()
              .ReverseMap();

            #endregion


            #region Wallet Address

            CreateMap<WalletAddress, WalletAddressDto>()
               .ReverseMap();

            CreateMap<PagedResults<WalletAddress>, PagedResultsDto<WalletAddressDto>>()
              .ReverseMap();

            #endregion


            #region Wallet

            CreateMap<WalletAddressBalance, WalletAddressBalanceDto>()
               .ReverseMap();

            CreateMap<PagedResults<WalletAddressBalance>, PagedResultsDto<WalletAddressBalanceDto>>()
              .ReverseMap();

            #endregion

            #region Whitelist Addresses

            CreateMap<WhitelistAddress, WhitelistAddressDto>()
               .ReverseMap();

            CreateMap<PagedResults<WhitelistAddress>, PagedResultsDto<WhitelistAddressDto>>()
               .ReverseMap();

            #endregion

            #region Instructions

            CreateMap<Instruction, StakingDepositInstructionDto>()
                .ForMember(dest => dest.Date, src => src.MapFrom(x => x.FromDate))
                .ForMember(dest => dest.Account, src => src.MapFrom(x => x.User))
                .ReverseMap();

            CreateMap<Instruction, StakingWithdrawalInstructionDto>()
                .ForMember(dest => dest.Date, src => src.MapFrom(x => x.FromDate))
                .ForMember(dest => dest.Account, src => src.MapFrom(x => x.User))
                .ReverseMap();

            CreateMap<Instruction, InstructionDto>()
              .ReverseMap();

            CreateMap<PagedResults<Instruction>, PagedResultsDto<InstructionDto>>()
               .ReverseMap();

            #endregion

            #region Staking

            CreateMap<StakingBalance, StakingBalanceDto>()
                .ReverseMap();

            CreateMap<PagedResults<StakingBalance>, PagedResultsDto<StakingBalanceDto>>()
                .ReverseMap();

            #endregion

            #region Misc

            CreateMap<Page, PageDto>()
               .ReverseMap();

            CreateMap<SortOrder, SortOrderDto>()
              .ReverseMap();

            #endregion
        }
    }
}
