CREATE TABLE [DeliusStaging].[RegistrationDetails](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[Date] DATE NULL,
	[TypeCode] NVARCHAR(10) NULL,
	[TypeDescription] NVARCHAR(50) NULL,
	[CategoryCode] NVARCHAR(100) NULL,
	[CategoryDescription] NVARCHAR(500) NULL,
	[RegisterCode] NVARCHAR(100) NULL,
	[RegisterDescription] NVARCHAR(500) NULL,
	[DeRegistered] NVARCHAR(1) NULL,
	[Deleted] NVARCHAR(1) NULL
) ON [PRIMARY]