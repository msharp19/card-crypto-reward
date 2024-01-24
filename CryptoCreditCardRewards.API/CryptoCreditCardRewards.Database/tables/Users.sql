CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Active] BIT NOT NULL DEFAULT(0),
	[Email] NVARCHAR(MAX) NOT NULL,
	[AccountNumber] NVARCHAR(MAX) NOT NULL,
	[CompletedKycDate] DATETIME NULL,
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    [CreatedById] INT NULL,
    [UpdatedDate] DATETIME NULL, 
    [UpdatedById]  INT NULL,
)
