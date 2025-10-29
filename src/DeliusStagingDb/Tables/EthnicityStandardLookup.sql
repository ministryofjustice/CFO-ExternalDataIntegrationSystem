CREATE TABLE [DeliusStaging].[Lookups.EthnicityStandard](
	[EthnicityID] [tinyint] NOT NULL,
	[StandardEthnicity] [nvarchar](100) NOT NULL,
	[SimpleEthnicity] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Lookups.EthnicityStandard] PRIMARY KEY CLUSTERED 
(
	[EthnicityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]