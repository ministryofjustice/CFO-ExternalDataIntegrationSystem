CREATE TABLE [DeliusRunningPicture].[AdditionalIdentifier](
	[OffenderId] BIGINT,
	[Id] BIGINT,
	CONSTRAINT [PK_AdditionalIdentifier] PRIMARY KEY (Id),
	[PNC] NVARCHAR(30) NULL,
	[YOT] NVARCHAR(30) NULL,
	[OldPnc] NVARCHAR(30) NULL,
	[MilitaryServiceNumber] NVARCHAR(30) NULL,
	[Deleted] NVARCHAR(1) NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END, 
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[AdditionalIdentifier]));