CREATE TABLE [DeliusRunningPicture].[OffenderTransfer](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT,
	CONSTRAINT [PK_OffenderTransfer] PRIMARY KEY (Id),
	[RequestDate] DATE NULL,
	[ReasonCode] NVARCHAR(100) NULL,
	[ReasonDescription] NVARCHAR(500) NULL,
	[StatusCode] NVARCHAR(100) NULL,
	[StatusDescription] NVARCHAR(500) NULL,
	[Deleted] NVARCHAR(1) NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[OffenderTransfer]))