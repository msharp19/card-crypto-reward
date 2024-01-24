CREATE TABLE [dbo].[WhitelistAddresses]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Address] NVARCHAR(MAX) NOT NULL,
	[ProcessedDate] DATETIME NULL,
	[Valid] BIT NOT NULL DEFAULT(0),
	[FailedReason] NVARCHAR(MAX) NULL,
	[CryptoCurrencyId] INT NOT NULL,
	[UserId] INT NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    [CreatedById] INT NULL,
    [UpdatedDate] DATETIME NULL, 
    [UpdatedById]  INT NULL, 

    CONSTRAINT [FK_WhitelistAddresses_ToUsers] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]), 
    CONSTRAINT [FK_WhitelistAddresses_ToCryptoCurrencies] FOREIGN KEY ([CryptoCurrencyId]) REFERENCES [CryptoCurrencies]([Id]),
)
