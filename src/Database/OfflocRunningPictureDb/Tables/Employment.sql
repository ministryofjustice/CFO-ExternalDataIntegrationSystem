CREATE TABLE [OfflocRunningPicture].[Employment]
(
	[NOMSnumber] NVARCHAR(7) NOT NULL,
	CONSTRAINT [PK_Employment] PRIMARY KEY (NOMSnumber),
	[Employed] CHAR(1) NULL,
	[EmploymentStatusReception] NVARCHAR(40) NULL,
	[EmploymentStatusDischarge] NVARCHAR(40) NULL,
	[IsActive] BIT NOT NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
)ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [OfflocTemporal].[Employment]))
