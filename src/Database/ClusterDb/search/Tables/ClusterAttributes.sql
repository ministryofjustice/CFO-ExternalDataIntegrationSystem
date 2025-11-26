CREATE TABLE [search].[ClusterAttributes] (
    [ClusterId]     INT            NOT NULL,
    [UPCI2]         NVARCHAR (9)   NOT NULL,
    [RecordSource]  NVARCHAR (50)  NOT NULL,
    [Identifier]    NVARCHAR (50)  NOT NULL,
    [PrimaryRecord] BIT            NOT NULL,
    [LastName]      NVARCHAR (100) NULL,
    [DOB]           DATE           NULL
);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20240612-140452]
    ON [search].[ClusterAttributes]([RecordSource] ASC, [LastName] ASC, [DOB] ASC)
    INCLUDE([UPCI2]);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20240612-140433]
    ON [search].[ClusterAttributes]([RecordSource] ASC, [Identifier] ASC)
    INCLUDE([UPCI2]);


GO
CREATE CLUSTERED INDEX [ClusteredIndex-20240612-140400]
    ON [search].[ClusterAttributes]([UPCI2] ASC);

