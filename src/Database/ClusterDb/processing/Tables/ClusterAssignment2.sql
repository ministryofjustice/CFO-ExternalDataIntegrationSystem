CREATE TABLE [processing].[ClusterAssignment2] (
    [SourceName]    NVARCHAR (50)    NOT NULL,
    [SourceKey]     NVARCHAR (50)    NOT NULL,
    [TargetName]    NVARCHAR (50)    NOT NULL,
    [TargetKey]     NVARCHAR (50)    NOT NULL,
    [Probability]   DECIMAL (18, 17) NOT NULL,
    [TempClusterId] INT              NOT NULL
);

