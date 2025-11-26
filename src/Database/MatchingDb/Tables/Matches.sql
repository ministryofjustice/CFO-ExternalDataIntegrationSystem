CREATE TABLE [Matching].[Matches](
	[CandidateId] [int] NOT NULL,
	[Probability] [decimal](18, 17) NULL,
	[JSON] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [Matching].[Matches]  WITH CHECK ADD  CONSTRAINT [FK_Matches_Candidates] FOREIGN KEY([CandidateId])
REFERENCES [Matching].[Candidates] ([CandidateId])
GO

ALTER TABLE [Matching].[Matches] CHECK CONSTRAINT [FK_Matches_Candidates]
GO