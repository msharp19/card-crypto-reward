using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Enums
{
    public enum FailedReason
    {
        AccountNumberIsNotUnique,
        EmailIsNotUnique,
        CurrencyDoesntExist,
        WalletAddressAlreadyExists,
        UserDoesntExist,
        WalletAddressDoesntExist,
        TotalContributionIsGreaterThan100,
        UserRewardSelectionDoesntExist,
        UserDoesntOwnRewardSelection,
        TotalContributionIsLessThan100,
        UserAlreadyDeactivated,
        UserAlreadyActive,
        SystemError,
        CryptoCurrencyDoesntSupportStaking,
        AmountMustBePossitive,
        KycIncomplete,
        AccountDoesntExist,
        UserAlreadyCompletedKyc,
        InfrastructureTypeNotSupported,
        NetworkIsntValid,
        UserDoesntOwnWallet,
        CryptoCurrencyDoesntExist,
        CryptoCurrencyIsCurrentlyInactive,
        AddressIsNotValid,
        GasPriceCouldNotBeDetermined,
        AmountExceedsBalancePlusGasPrice,
        TrasactionDoesntExist,
        TrasactionDoesntExistOrIsFor0Amount,
        TransactionAlreadyImported,
        TransactionIsNotForThisWalletAddress,
        NoSystemWalletExistsForStakingThisCurrency,
        InstructionNotFound,
        RewardSpendBandDoesntExist,
        BandToMustBeGreaterThanBandFrom,
        BandToMustBeGreaterThan0,
        RangeCrossesAnExistingRange,
        PercentageRewardMustBeGreaterThan0,
        WhitelistAddressDoesntExist,
        WhitelistAddressNotValid,
        AddressAlreadyWhitelisted,
        CantUnStakeMoreThanActuallyStaked,
        AmountExceedsSystemWalletBalancePlusGasPrice,
        SymbolNotSupportedForConversionImplementation,
        AddressNotWhitelisted
    }
}
