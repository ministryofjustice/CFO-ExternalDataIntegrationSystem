CREATE TABLE [DeliusStaging].[AdditionalIdentifier](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[PNC] NVARCHAR(30) NULL,
	[YOT] NVARCHAR(30) NULL,
	[OldPnc] NVARCHAR(30) NULL,
	[MilitaryServiceNumber] NVARCHAR(30) NULL,
	[Deleted] NVARCHAR(1) NULL
) ON [PRIMARY]