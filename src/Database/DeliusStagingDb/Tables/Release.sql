CREATE TABLE [dbo].[Release](
	[OffenderId] [bigint] NOT NULL,
	[Id] [bigint] NOT NULL,
	[ReleaseDate] [datetime] NULL,
	[ReleaseCode] [nvarchar](100) NULL,
	[ReleaseDescription] [nvarchar](500) NULL,
	[Deleted] [nvarchar](1) NULL,
	[LastUpdated] [datetime] NULL
) ON [PRIMARY]