CREATE TABLE [dbo].[CryptoCurrencies]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Active] BIT NOT NULL DEFAULT(0),
	[Name] NVARCHAR(MAX) NOT NULL,
	[IsTestNetwork] BIT NOT NULL,
	[ConversionServiceType] NVARCHAR(256) NOT NULL,
	[SupportsStaking] BIT NOT NULL DEFAULT(0),
	[InfrastructureType] NVARCHAR(256) NOT NULL,
	[NetworkEndpoint] NVARCHAR(MAX) NOT NULL,
	[Symbol] NVARCHAR(256) NOT NULL,
	[TestNetwork] BIT NOT NULL DEFAULT(0),
	[Description] NVARCHAR(MAX) NULL,
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    [CreatedById] INT NULL,
    [UpdatedDate] DATETIME NULL, 
    [UpdatedById]  INT NULL,
)
