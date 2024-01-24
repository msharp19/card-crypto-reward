CREATE TABLE [dbo].[WalletAddresses]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Active] BIT NOT NULL DEFAULT(0),
	[Address] NVARCHAR(256) NOT NULL,
	[KeyData] NVARCHAR(MAX) NOT NULL,
	[UserId] INT NOT NULL,
	[CryptoCurrencyId] INT NOT NULL,
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    [CreatedById] INT NULL,
    [UpdatedDate] DATETIME NULL, 
    [UpdatedById]  INT NULL, 
    CONSTRAINT [FK_WalletAddresses_ToCryptoCurrencies] FOREIGN KEY ([CryptoCurrencyId]) REFERENCES [CryptoCurrencies]([Id]), 
    CONSTRAINT [FK_WalletAddresses_ToUsers] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
)
