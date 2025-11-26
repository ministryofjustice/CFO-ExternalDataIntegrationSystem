CREATE TABLE [DeliusStaging].[OffenderTransfer](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[RequestDate] DATE NULL,
	[ReasonCode] NVARCHAR(100) NULL,
	[ReasonDescription] NVARCHAR(500) NULL,
	[StatusCode] NVARCHAR(100) NULL,
	[StatusDescription] NVARCHAR(500) NULL,
	[Deleted] NVARCHAR(1) NULL
) ON [PRIMARY]