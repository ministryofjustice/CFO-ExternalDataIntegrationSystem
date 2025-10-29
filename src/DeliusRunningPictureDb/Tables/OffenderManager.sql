CREATE TABLE [DeliusRunningPicture].[OffenderManager](
	[OmCode] NVARCHAR(7),
	[OmForename] NVARCHAR(35) NULL,
	[OmSurname] NVARCHAR(35) NULL,
	[OrgCode] NVARCHAR(3) NULL,
	[TeamCode] NVARCHAR(6),
	[ContactNo] NVARCHAR(35) NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo]), 
	CONSTRAINT [PK_OffenderManager] PRIMARY KEY (OmCode, TeamCode),
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[OffenderManager]));