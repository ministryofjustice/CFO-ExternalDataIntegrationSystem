CREATE TABLE [processing].[ClusterAssignment] (
    [SourceName]    NVARCHAR (50)    NOT NULL,
    [SourceKey]     NVARCHAR (50)    NOT NULL,
    [TargetName]    NVARCHAR (50)    NOT NULL,
    [TargetKey]     NVARCHAR (50)    NOT NULL,
    [Probability]   DECIMAL (18, 17) NOT NULL,
    [TempClusterId] INT              NOT NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex-20240611-123944]
    ON [processing].[ClusterAssignment]([TargetName] ASC, [TargetKey] ASC, [SourceName] ASC, [SourceKey] ASC)
    INCLUDE([Probability]);


GO
CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex-20240611-123856]
    ON [processing].[ClusterAssignment]([SourceName] ASC, [SourceKey] ASC, [TargetName] ASC, [TargetKey] ASC);

