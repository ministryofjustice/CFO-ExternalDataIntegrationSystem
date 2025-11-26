CREATE TABLE [OfflocRunningPicture].[Bookings]
(
	[NOMSnumber] NVARCHAR(7) NOT NULL,
	[PrisonNumber] NVARCHAR(6),
	[FirstReceptionDate] DATE NOT NULL,
	[IsActive] BIT NOT NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo]),
	CONSTRAINT [PK_Bookings] PRIMARY KEY (NOMSnumber, PrisonNumber)
)ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [OfflocTemporal].[Bookings]))
