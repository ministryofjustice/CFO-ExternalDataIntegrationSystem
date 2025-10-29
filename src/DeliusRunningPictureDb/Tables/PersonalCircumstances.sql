CREATE TABLE [DeliusRunningPicture].[PersonalCircumstances](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT,
	CONSTRAINT [PK_PersonalCircumstances] PRIMARY KEY (Id),
	[Type] NVARCHAR(100) NULL,
	[TypeDescription] NVARCHAR(500) NULL,
	[SubType] NVARCHAR(100) NULL,
	[SubTypeDescription] NVARCHAR(500) NULL,
	[StartDate] DATE NULL,
	[EndDate] DATE NULL,
	[Deleted] NVARCHAR(1) NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[PersonalCircumstances]));