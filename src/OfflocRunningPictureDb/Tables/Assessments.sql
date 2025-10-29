CREATE TABLE [OfflocRunningPicture].[Assessments]
(
	[NOMSnumber] NVARCHAR(7) NOT NULL,
	CONSTRAINT [PK_Assessments] PRIMARY KEY (NOMSnumber),
	[SecurityCategory] NVARCHAR(30) NULL,
	[DateSecurityCategoryReview] DATE NULL,
	[DateSecCatChanged] DATE NULL,
	[IsActive] BIT NOT NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
)ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [OfflocTemporal].[Assessments]))
