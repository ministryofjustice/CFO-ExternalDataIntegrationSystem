CREATE TABLE [DeliusRunningPicture].[OffenderToOffenderManagerMappings]
(
	[OffenderId] BIGINT,
	[Id] BIGINT,
	[AllocatedDate] DATE NULL,
	[EndDate] DATE NULL,
	[OmCode] NVARCHAR(7),
	[OrgCode] [nvarchar](3) NULL,
	[TeamCode] [nvarchar](6) NULL,
	[Deleted] NVARCHAR(1) NULL,
	CONSTRAINT [PK_OffenderToOffenderManagerMappings] PRIMARY KEY (OffenderId, Id, OmCode),
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[OffenderToOffenderManagerMappings]));
