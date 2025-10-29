CREATE TABLE [processing].[ClusterMeta] (
    [TempClusterId] INT           NOT NULL,
    [NodeSource]    NVARCHAR (50) NOT NULL,
    [NodeKey]       NVARCHAR (50) NOT NULL,
    [NodeId]        INT           NOT NULL,
    [NodeCount]     INT           NOT NULL
);

