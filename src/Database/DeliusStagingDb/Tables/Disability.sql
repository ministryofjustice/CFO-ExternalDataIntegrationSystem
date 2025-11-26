CREATE TABLE [DeliusStaging].[Disability](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[TypeCode] NVARCHAR(100) NULL,
	[TypeDescription] NVARCHAR(500) NULL,
	[StartDate] DATE NULL,
	[EndDate] DATE NULL,
	[Deleted] NVARCHAR(1) NULL
) ON [PRIMARY]