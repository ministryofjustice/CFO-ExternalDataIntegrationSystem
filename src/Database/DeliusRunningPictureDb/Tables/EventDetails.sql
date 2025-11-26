CREATE TABLE [DeliusRunningPicture].[EventDetails](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT,
	CONSTRAINT [PK_EventDetails] PRIMARY KEY (Id),
	[ReferralDate] DATE NULL,
	[ConvictionDate] DATE NULL,
	[Cohort] NVARCHAR(11) NULL,
	[Deleted] NVARCHAR(1) NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[EventDetails]));