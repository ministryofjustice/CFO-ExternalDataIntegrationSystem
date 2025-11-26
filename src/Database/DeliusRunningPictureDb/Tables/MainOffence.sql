CREATE TABLE [DeliusRunningPicture].[MainOffence](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT,
	CONSTRAINT [PK_MainOffence] PRIMARY KEY (Id),
	[EventId] BIGINT NOT NULL,
	[OffenceCode] NVARCHAR(5) NULL,
	[OffenceDescription] NVARCHAR(300) NULL,
	[OffenceDate] DATE NULL,
	[Deleted] NVARCHAR(1) NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[MainOffence]));


GO
CREATE NONCLUSTERED INDEX [idx_mainoffence_offenderid]
    ON [DeliusRunningPicture].[MainOffence]([OffenderId] ASC);
