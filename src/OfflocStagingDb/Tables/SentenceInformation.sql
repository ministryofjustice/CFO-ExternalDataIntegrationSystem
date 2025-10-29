CREATE TABLE [OfflocStaging].[SentenceInformation]
(
	[NOMSnumber] NVARCHAR(7) NOT NULL,
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
	[tused] DATE NULL
)ON [PRIMARY]
