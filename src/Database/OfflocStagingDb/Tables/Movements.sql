CREATE TABLE [OfflocStaging].[Movements]
(
	[NOMSnumber] NVARCHAR(7) NOT NULL,
	[MovementEstabComponent] NVARCHAR(38) NULL,
	[MovementCode] NVARCHAR(10) NULL,
	[TransferReason] NVARCHAR(35) NULL,
	[DateOfMovement] DATE NULL,
	[HourOfMovement] INT NULL,
	[MinOfMovement] INT NULL,
	[SecOfMovement] INT NULL
)ON [PRIMARY]
