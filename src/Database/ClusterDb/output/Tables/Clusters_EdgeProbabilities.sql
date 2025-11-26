CREATE TABLE [output].[Clusters_EdgeProbabilities] (
    [TempClusterId] INT              NOT NULL,
    [SourceName]    NVARCHAR (50)    NOT NULL,
    [SourceKey]     NVARCHAR (50)    NOT NULL,
    [TargetName]    NVARCHAR (50)    NOT NULL,
    [TargetKey]     NVARCHAR (50)    NOT NULL,
    [Probability]   DECIMAL (18, 17) NOT NULL
);

