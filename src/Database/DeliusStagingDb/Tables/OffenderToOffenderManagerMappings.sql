CREATE TABLE [DeliusStaging].[OffenderToOffenderManagerMappings]
(
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[AllocatedDate] SMALLDATETIME NULL,
	[EndDate] DATE NULL,
	[OmCode] NVARCHAR(7) NULL,
	[OrgCode] [nvarchar](3) NULL,
	[TeamCode] [nvarchar](6) NULL,
	[Deleted] NVARCHAR(1) NULL
)
