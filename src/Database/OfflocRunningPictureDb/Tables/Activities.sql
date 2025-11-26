CREATE TABLE [OfflocRunningPicture].[Activities](
	[NOMSnumber] NVARCHAR(7) NOT NULL,
	[Activity] NVARCHAR(50),
	[Location] NVARCHAR(50) NOT NULL,
	[StartHour] INT NOT NULL,
	[StartMin] INT NOT NULL,
	[EndHour] INT NOT NULL,
	[EndMin] INT NOT NULL,
	[IsActive] BIT NOT NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo]),
	CONSTRAINT [PK_Activities] PRIMARY KEY ([NOMSnumber], [Activity], [Location], [StartHour], [StartMin], [EndHour], [EndMin]),
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [OfflocTemporal].[Activities]))
