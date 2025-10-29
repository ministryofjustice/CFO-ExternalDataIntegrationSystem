CREATE TABLE [OfflocRunningPicture].[SentenceInformation]
(
	[NOMSnumber] NVARCHAR(7) NOT NULL,
	CONSTRAINT [PK_SentenceInformation] PRIMARY KEY (NOMSnumber),
	[FirstSentenced] DATE NULL,
	[SentenceYears] INT NULL,
	[SentenceMonths] INT NULL,
	[SentenceDays] INT NULL,
	[EarliestPossibleReleaseDate] DATE NULL,
	[DateOfRelease] DATE NULL,
	[sed] NVARCHAR(10) NULL,
	[hdced] NVARCHAR(10) NULL,
	[hdcad] NVARCHAR(10) NULL,
	[ped] NVARCHAR(10) NULL,
	[crd] NVARCHAR(10) NULL,
	[npd] NVARCHAR(10) NULL,
	[led] NVARCHAR(10) NULL,
	[tused] DATE NULL,
	[IsActive] BIT NOT NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
)ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [OfflocTemporal].[SentenceInformation]))
