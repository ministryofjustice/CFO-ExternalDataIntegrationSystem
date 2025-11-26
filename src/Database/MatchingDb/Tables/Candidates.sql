CREATE TABLE [Matching].[Candidates] (
    [CandidateId]   INT           IDENTITY (1, 1) NOT NULL,
    [BlockId]       INT           NOT NULL,
    [l_SourceName]  NVARCHAR (16) NOT NULL,
    [l_SourceKey]   NVARCHAR (16) NOT NULL,
    [l_NOMISNumber] NVARCHAR (7)  NULL,
    [l_PNCNumber]   NVARCHAR (15) NULL,
    [l_FirstName]   NVARCHAR (50) NULL,
    [l_SecondName]  NVARCHAR (50) NULL,
    [l_LastName]    NVARCHAR (50) NULL,
    [l_DateOfBirth] DATE          NULL,
    [l_CRONumber]   NVARCHAR (35) NULL,
    [l_Gender]      NVARCHAR (50) NULL,
    [r_SourceName]  NVARCHAR (16) NOT NULL,
    [r_SourceKey]   NVARCHAR (16) NOT NULL,
    [r_NOMISNumber] NVARCHAR (7)  NULL,
    [r_PNCNumber]   NVARCHAR (15) NULL,
    [r_FirstName]   NVARCHAR (50) NULL,
    [r_SecondName]  NVARCHAR (50) NULL,
    [r_LastName]    NVARCHAR (50) NULL,
    [r_DateOfBirth] DATE          NULL,
    [r_CRONumber]   NVARCHAR (35) NULL,
    [r_Gender]      NVARCHAR (50) NULL,
    CONSTRAINT [PK_Candidates] PRIMARY KEY CLUSTERED ([CandidateId] ASC),
    CONSTRAINT [FK_Candidates_Blocks] FOREIGN KEY ([BlockId]) REFERENCES [Matching].[Blocks] ([BlockId])
);


GO


GO


GO