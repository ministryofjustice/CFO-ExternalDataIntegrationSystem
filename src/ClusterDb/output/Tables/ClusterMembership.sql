CREATE TABLE [output].[ClusterMembership] (
    [ClusterId]                    INT              NULL,
    [NodeName]                     NVARCHAR (50)    NOT NULL,
    [NodeKey]                      NVARCHAR (50)    NOT NULL,
    [ClusterMembershipProbability] DECIMAL (18, 17) NULL,
    [HardLink]                     BIT              NULL
);


GO
CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex-20240611-145416]
    ON [output].[ClusterMembership]([NodeName] ASC, [NodeKey] ASC);

