CREATE TABLE [DeliusStaging].[Requirement](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[DisposalId] BIGINT NOT NULL,
	[StartDate] DATE NULL,
	[CommencementDate] DATE NULL,
	[Terminationdate] DATE NULL,
	[TerminationReasonCode] NVARCHAR(100) NULL,
	[TerminationDescription] NVARCHAR(500) NULL,
	[CategoryCode] NVARCHAR(20) NULL,
	[CategoryDescription] NVARCHAR(200) NULL,
	[SubCategoryCode] NVARCHAR(100) NULL,
	[SubCategoryDescription] NVARCHAR(500) NULL,
	[Length] NVARCHAR(38) NULL,
	[UnitCode] NVARCHAR(100) NULL,
	[UnitDescription] NVARCHAR(500) NULL,
	[Deleted] NVARCHAR(1) NULL
) ON [PRIMARY]