CREATE TABLE [DeliusRunningPicture].[Requirement](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT,
	CONSTRAINT [PK_Requirement] PRIMARY KEY (Id),
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
	[Deleted] NVARCHAR(1) NULL,
	[ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[Requirement]));


GO
CREATE NONCLUSTERED INDEX [idx_requirement_disposalid]
    ON [DeliusRunningPicture].[Requirement]([DisposalId] ASC);
