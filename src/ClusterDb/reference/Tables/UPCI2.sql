CREATE TABLE [reference].[UPCI2] (
    [ClusterId] INT          IDENTITY (1, 1) NOT NULL,
    [UPCI2]     NVARCHAR (9) NOT NULL
);


GO
CREATE UNIQUE CLUSTERED INDEX [ClusteredIndex-20240612-141714]
    ON [reference].[UPCI2]([ClusterId] ASC);

