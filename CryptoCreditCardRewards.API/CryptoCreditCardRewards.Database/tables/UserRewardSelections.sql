CREATE TABLE [dbo].[UserRewardSelections]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Active] BIT NOT NULL DEFAULT(0),
	[UserId] INT NOT NULL,
	[CryptoCurrencyId] INT NOT NULL,
	[ContributionPercentage] DECIMAL(18,8) NOT NULL,
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    [CreatedById] INT NULL,
    [UpdatedDate] DATETIME NULL, 
    [UpdatedById]  INT NULL, 
    CONSTRAINT [FK_UserRewardSelections_ToUsers] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]), 
    CONSTRAINT [FK_UserRewardSelections_ToCryptoCurrencies] FOREIGN KEY ([CryptoCurrencyId]) REFERENCES [CryptoCurrencies]([Id]),
)
