CREATE TABLE [DeliusRunningPicture].[RegistrationDetails](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT,
	CONSTRAINT [PK_RegistrationDetails] PRIMARY KEY (Id),
	[Date] DATE NULL,
	[TypeCode] NVARCHAR(10) NULL,
	[TypeDescription] NVARCHAR(50) NULL,
	[CategoryCode] NVARCHAR(100) NULL,
	[CategoryDescription] NVARCHAR(500) NULL,
	[RegisterCode] NVARCHAR(100) NULL,
	[RegisterDescription] NVARCHAR(500) NULL,
	[DeRegistered] NVARCHAR(1) NULL,
	[Deleted] NVARCHAR(1) NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[RegistrationDetails]));


GO
CREATE NONCLUSTERED INDEX [idx_RegistrationDetails_OffenderId]
    ON [DeliusRunningPicture].[RegistrationDetails]([OffenderId] ASC);