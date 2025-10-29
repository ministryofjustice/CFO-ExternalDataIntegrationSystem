CREATE TABLE [DeliusRunningPicture].[OffenderManagerBuildings]
(
	[CompositeHash] VARBINARY(900),
	[BuildingName] NVARCHAR(35) NULL,
	[PostCode] NVARCHAR(8) NULL,
	[HouseNumber] NVARCHAR(35) NULL,
	[Street] NVARCHAR(35) NULL,
	[District] NVARCHAR(35) NULL,
	[Town] NVARCHAR(35) NULL,
	[County] NVARCHAR(35) NULL,
	CONSTRAINT [PK_OffenderManagerBuildings] PRIMARY KEY(CompositeHash),
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[OffenderManagerBuildings]));
