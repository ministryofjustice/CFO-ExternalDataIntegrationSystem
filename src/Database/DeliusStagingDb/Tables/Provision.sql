CREATE TABLE [DeliusStaging].[Provision](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[Code] NVARCHAR(100) NULL,
	[Description] NVARCHAR(500) NULL,
	[StartDate] DATE NULL,
	[EndDate] DATE NULL,
	[Deleted] NVARCHAR(1) NULL
) ON [PRIMARY]