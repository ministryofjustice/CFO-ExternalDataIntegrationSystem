CREATE TABLE [OfflocRunningPicture].[Movements]
(
	[NOMSnumber] NVARCHAR(7) NOT NULL,
	CONSTRAINT [PK_Movements] PRIMARY KEY (NOMSnumber),
	[MovementEstabComponent] NVARCHAR(38) NULL,
	[MovementCode] NVARCHAR(10) NULL,
	[TransferReason] NVARCHAR(35) NULL,
	[DateOfMovement] DATE NULL,
	[HourOfMovement] INT NULL,
	[MinOfMovement] INT NULL,
	[SecOfMovement] INT NULL,
	[IsActive] BIT NOT NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
)ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [OfflocTemporal].[Movements]))
