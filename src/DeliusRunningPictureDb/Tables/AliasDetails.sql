CREATE TABLE [DeliusRunningPicture].[AliasDetails](
	[RowNo] INT IDENTITY(0, 1),
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT,
	CONSTRAINT [PK_AliasDetails] PRIMARY KEY (Id),
	[FirstName] NVARCHAR(35) NULL,
	[SecondName] NVARCHAR(35) NULL,
	[ThirdName] NVARCHAR(35) NULL,
	[Surname] NVARCHAR(35) NULL,
	[DateOfBirth] DATE NULL,
	[GenderCode] NVARCHAR(100) NULL,
	[GenderDescription] NVARCHAR(500) NULL,
	[Deleted] NVARCHAR(1) NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[AliasDetails]));