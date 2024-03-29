﻿CREATE TABLE [dbo].[Transactions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Active] BIT NOT NULL DEFAULT(0),
	[Type] NVARCHAR(256) NOT NULL,
	[State] NVARCHAR(256) NOT NULL DEFAULT('Pending'),
	[ConfirmedDate] DATETIME NULL,
	[ReviewedDate] DATETIME NULL,
	[ReviewedNotes] NVARCHAR(MAX) NULL,
	[SystemWalletAddressId] INT NULL,
	[WhitelistAddressId] INT NULL,
	[WalletAddressId] INT NOT NULL,
	[FailedReview] BIT NOT NULL DEFAULT(0),
    [Hash] NVARCHAR(MAX) NOT NULL,
	[CryptoCurrencyId] INT NOT NULL,
	[FromAddress] NVARCHAR(MAX) NULL,
	[ToAddress] NVARCHAR(MAX) NULL,
	[UserId] INT NOT NULL,
	[InstructionId] INT NULL,
	[Amount] DECIMAL(18,8) NOT NULL DEFAULT(0),
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    [CreatedById] INT NULL,
    [UpdatedDate] DATETIME NULL, 
    [UpdatedById]  INT NULL, 
    CONSTRAINT [FK_Transactions_ToCryptoCurrencies] FOREIGN KEY ([CryptoCurrencyId]) REFERENCES [CryptoCurrencies]([Id]), 
    CONSTRAINT [FK_Transactions_ToUsers] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
	CONSTRAINT [FK_Transactions_ToInstructions] FOREIGN KEY ([InstructionId]) REFERENCES [Instructions]([Id]), 
    CONSTRAINT [FK_Transactions_ToSystemWalletAddresses] FOREIGN KEY ([SystemWalletAddressId]) REFERENCES [SystemWalletAddresses]([Id]),
	CONSTRAINT [FK_Transactions_ToWalletAddresses] FOREIGN KEY ([WalletAddressId]) REFERENCES [WalletAddresses]([Id]), 
    CONSTRAINT [FK_Transactions_ToWhitelistAddresses] FOREIGN KEY ([WhitelistAddressId]) REFERENCES [WhitelistAddresses]([Id]),
)
