CREATE TABLE [DeliusStaging].[EventDetails](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[ReferralDate] DATE NULL,
	[ConvictionDate] DATE NULL,
	[Cohort] NVARCHAR(11) NULL,
	[Deleted] NVARCHAR(1) NULL
) ON [PRIMARY]