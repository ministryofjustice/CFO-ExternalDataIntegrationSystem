CREATE TABLE [DeliusRunningPicture].[OffenderManagerTeam]
(
	[OrgCode] NVARCHAR(3),
	[OrgDescription] NVARCHAR(60) NULL,
	[TeamCode] NVARCHAR(6),
	[TeamDescription] NVARCHAR(50) NULL,
	CONSTRAINT [PK_OffenderManagerTeam] PRIMARY KEY(OrgCode, TeamCode, CompositeBuildingHash),
	[CompositeBuildingHash] VARBINARY(32),
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[OffenderManagerTeam]));
