CREATE TABLE [input].[Nodes] (
    [NodeName] NVARCHAR (50) NOT NULL,
    [NodeKey]  NVARCHAR (50) NOT NULL
);


GO
CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex-20240611-141243]
    ON [input].[Nodes]([NodeName] ASC, [NodeKey] ASC);

