CREATE TABLE [OfflocRunningPicture].[Agencies]
(
	[EstablishmentCode] NVARCHAR(3) ,
	CONSTRAINT [PK_Agencies] PRIMARY KEY (EstablishmentCode),
	[Establishment] NVARCHAR(40) NOT NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
)ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [OfflocTemporal].[Agencies]))
