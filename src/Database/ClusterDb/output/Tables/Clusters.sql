CREATE TABLE [output].[Clusters] (
    [ClusterId]                     INT           NOT NULL,
    [UPCI2]                         NVARCHAR (9)  NOT NULL,
    [RecordCount]                   SMALLINT      NULL,
    [ContainsInternalDupe]          BIT           NULL,
    [ContainsLowProbabilityMembers] BIT           NULL,
    [PrimaryRecordName]             NVARCHAR (50) NULL,
    [PrimaryRecordKey]              NVARCHAR (50) NULL, 
    [IdentifiedOn] DATETIME2 NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex-20240612-123941]
    ON [output].[Clusters]([ClusterId] ASC)
    INCLUDE([UPCI2]);


GO
CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex-20240612-123922]
    ON [output].[Clusters]([UPCI2] ASC);

