CREATE TABLE [DeliusStaging].[OffenderManagerTeam]
(
	[OrgCode] NVARCHAR(3) NULL,
	[OrgDescription] NVARCHAR(60) NULL,
	[TeamCode] NVARCHAR(6) NULL,
	[TeamDescription] NVARCHAR(50) NULL,
	[CompositeBuildingHash] VARBINARY(32) NOT NULL,
)
