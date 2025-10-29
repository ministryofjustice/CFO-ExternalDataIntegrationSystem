CREATE TABLE [DeliusStaging].[OffenderAddress](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[StatusCode] NVARCHAR(100) NULL,
	[StatusDescription] NVARCHAR(500) NULL,
	[BuildingName] NVARCHAR(35) NULL,
	[HouseNumber] NVARCHAR(35) NULL,
	[StreetName] NVARCHAR(35) NULL,
	[District] NVARCHAR(35) NULL,
	[Town] NVARCHAR(35) NULL,
	[County] NVARCHAR(35) NULL,
	[Postcode] NVARCHAR(8) NULL,
	[StartDate] DATE NULL,
	[NoFixedAbode] NVARCHAR(1) NULL,
	[Deleted] NVARCHAR(1) NULL
) ON [PRIMARY]