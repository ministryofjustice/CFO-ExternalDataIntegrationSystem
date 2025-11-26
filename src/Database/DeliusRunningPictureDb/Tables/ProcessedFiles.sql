CREATE TABLE [DeliusRunningPicture].[ProcessedFiles] 
(
    [FileName]  VARCHAR (50)                                NOT NULL,
    [FileId]    INT                                         NOT NULL,
    [ValidFrom] DATETIME2 (7) GENERATED ALWAYS AS ROW START DEFAULT (sysutcdatetime()) NOT NULL,
    [ValidTo]   DATETIME2 (7) GENERATED ALWAYS AS ROW END   DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) NOT NULL,
    [Status]    VARCHAR (10)                                NULL,
    CONSTRAINT [PK_ProcessedFiles] PRIMARY KEY ([FileId] ASC),
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [DeliusTemporal].[ProcessedFiles]))
