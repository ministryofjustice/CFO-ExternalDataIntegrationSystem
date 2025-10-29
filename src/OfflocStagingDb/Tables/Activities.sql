CREATE TABLE [OfflocStaging].[Activities](
	[NOMSnumber] NVARCHAR(7) NOT NULL,
	[Activity] NVARCHAR(50) NOT NULL,
	[Location] NVARCHAR(50) NOT NULL,
	[StartHour] INT NOT NULL,
	[StartMin] INT NOT NULL,
	[EndHour] INT NOT NULL,
	[EndMin] INT NOT NULL
) ON [PRIMARY]