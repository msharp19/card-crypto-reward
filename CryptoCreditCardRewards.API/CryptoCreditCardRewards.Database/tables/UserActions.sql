CREATE TABLE [dbo].[UserActions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[ActionType] NVARCHAR(256) NOT NULL,
	[TransactionId] INT NULL,
	[WalletAddressId] INT NULL,
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    [CreatedById] INT NULL,
    [UpdatedDate] DATETIME NULL, 
    [UpdatedById]  INT NULL, 
    CONSTRAINT [FK_UserActions_ToTransactions] FOREIGN KEY ([TransactionId]) REFERENCES [Transactions]([Id]), 
    CONSTRAINT [FK_UserActions_ToWalletAddresses] FOREIGN KEY ([WalletAddressId]) REFERENCES [WalletAddresses]([Id]),
)
