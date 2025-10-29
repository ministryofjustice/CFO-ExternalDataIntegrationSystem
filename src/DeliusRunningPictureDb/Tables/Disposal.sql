CREATE TABLE [DeliusRunningPicture].[Disposal](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	CONSTRAINT [PK_Disposal] PRIMARY KEY ([Id]),
	[EventId] BIGINT,	
	[SentenceDate] DATE NULL,
	[Length] NVARCHAR(38) NULL,
	[UnitCode] NVARCHAR(100) NULL,
	[UnitDescription] NVARCHAR(500) NULL,
	[DisposalCode] NVARCHAR(30) NULL,
	[DisposalDetail] NVARCHAR(128) NULL,
	[DisposalTerminationCode] NVARCHAR(100) NULL,
	[DisposalTerminationDescription] NVARCHAR(500) NULL,
	[TerminationDate] DATE  NULL,
	[Deleted] NVARCHAR(1) NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[Disposal]));


GO
CREATE NONCLUSTERED INDEX [idx_disposal_eventid]
    ON [DeliusRunningPicture].[Disposal]([EventId] ASC);