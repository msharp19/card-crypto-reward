CREATE TABLE [dbo].[SystemWalletAddresses]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Active] BIT NOT NULL DEFAULT(0),
	[AddressType] NVARCHAR(256) NOT NULL,
	[Address] NVARCHAR(256) NOT NULL,
	[KeyData] NVARCHAR(MAX) NOT NULL,
	[CryptoCurrencyId] INT NOT NULL,
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    [CreatedById] INT NULL,
    [UpdatedDate] DATETIME NULL, 
    [UpdatedById]  INT NULL, 
    CONSTRAINT [FK_SystemWalletAddresses_ToCryptoCurrencies] FOREIGN KEY ([CryptoCurrencyId]) REFERENCES [CryptoCurrencies]([Id]),
)
