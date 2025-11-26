CREATE TABLE [DeliusStaging].[AliasDetails](
	[OffenderId] BIGINT NOT NULL,
	[Id] BIGINT NOT NULL,
	[FirstName] NVARCHAR(35) NULL,
	[SecondName] NVARCHAR(35) NULL,
	[ThirdName] NVARCHAR(35) NULL,
	[Surname] NVARCHAR(35) NULL,
	[DateOfBirth] DATE NULL,
	[GenderCode] NVARCHAR(100) NULL,
	[GenderDescription] NVARCHAR(500) NULL,
	[Deleted] NVARCHAR(1) NULL
) ON [PRIMARY]