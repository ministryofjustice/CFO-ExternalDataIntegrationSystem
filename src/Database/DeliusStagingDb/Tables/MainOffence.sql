CREATE TABLE [DeliusStaging].[MainOffence](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[EventId] BIGINT NOT NULL,
	[OffenceCode] NVARCHAR(5) NULL,
	[OffenceDescription] NVARCHAR(300) NULL,
	[OffenceDate] DATE NULL,
	[Deleted] NVARCHAR(1) NULL
) ON [PRIMARY]