CREATE TABLE [DeliusStaging].[Disposal](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[EventId] BIGINT NOT NULL,
	[SentenceDate] DATE NULL,
	[Length] NVARCHAR(38) NULL,
	[UnitCode] NVARCHAR(100) NULL,
	[UnitDescription] NVARCHAR(500) NULL,
	[DisposalCode] NVARCHAR(30) NULL,
	[DisposalDetail] NVARCHAR(128) NULL,
	[DisposalTerminationCode] NVARCHAR(100) NULL,
	[DisposalTerminationDescription] NVARCHAR(500) NULL,
	[TerminationDate] DATE NULL,
	[Deleted] NVARCHAR(1) NULL
) ON [PRIMARY]