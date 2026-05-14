CREATE TABLE [dbo].[FailedStagingRows]
(
    [Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Timestamp]       DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
    [FileName]        NVARCHAR(256)    NOT NULL,
    [FilePath]        NVARCHAR(1024)   NOT NULL,
    [TableReference]  NVARCHAR(384)    NOT NULL,  -- Database.Schema.Table
    [FailureType]     NVARCHAR(32)     NOT NULL,  -- 'ParseFailure' | 'InsertFailure'
    [LineNumber]      INT              NULL,       -- populated for ParseFailure, null for InsertFailure
    [RowContent]      NVARCHAR(MAX)    NOT NULL,
    [ErrorMessage]    NVARCHAR(MAX)    NOT NULL,
    [ErrorDetail]     NVARCHAR(MAX)    NULL,       -- full stack trace
    CONSTRAINT [PK_FailedStagingRows] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_FailedStagingRows_FileName]        ON [dbo].[FailedStagingRows] ([FileName]);
GO

CREATE INDEX [IX_FailedStagingRows_TableReference]  ON [dbo].[FailedStagingRows] ([TableReference]);
GO

CREATE INDEX [IX_FailedStagingRows_FailureType]     ON [dbo].[FailedStagingRows] ([FailureType]);
GO
