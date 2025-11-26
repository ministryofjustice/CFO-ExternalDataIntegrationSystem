CREATE TABLE [DeliusStaging].[PersonalCircumstances](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[Type] NVARCHAR(100) NULL,
	[TypeDescription] NVARCHAR(500) NULL,
	[SubType] NVARCHAR(100) NULL,
	[SubTypeDescription] NVARCHAR(500) NULL,
	[StartDate] DATE NULL,
	[EndDate] DATE NULL,
	[Deleted] NVARCHAR(1) NULL
) ON [PRIMARY]