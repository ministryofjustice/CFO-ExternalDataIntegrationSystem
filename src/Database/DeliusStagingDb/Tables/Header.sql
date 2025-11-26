CREATE TABLE [DeliusStaging].[Header](
	[OffenderId] BIGINT NOT NULL,
	[FileType] NVARCHAR(4) NULL,
	[Sequence] BIGINT NOT NULL,
	[RunDate] DATETIME2 NULL,
	[SectionCount] BIGINT NOT NULL
) ON [PRIMARY]