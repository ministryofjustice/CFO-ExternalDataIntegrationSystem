CREATE TABLE [DeliusStaging].[OffenderManagerBuildings]
(
	[CompositeHash] VARBINARY(32) NOT NULL, 
	[BuildingName] NVARCHAR(35) NULL,
	[PostCode] NVARCHAR(8) NULL,
	[HouseNumber] NVARCHAR(35) NULL,
	[Street] NVARCHAR(35) NULL,
	[District] NVARCHAR(35) NULL,
	[Town] NVARCHAR(35) NULL,
	[County] NVARCHAR(35) NULL
)
