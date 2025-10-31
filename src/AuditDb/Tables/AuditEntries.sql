CREATE TABLE [dbo].[AuditEntries]
(
    [Id] uniqueidentifier NOT NULL,
    [CorrelationId] uniqueidentifier NOT NULL,
    [EntityName] nvarchar(256) NOT NULL,
    [Action] nvarchar(32) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [PerformedBy] nvarchar(256) NULL,
    [KeyValues] nvarchar(max) NOT NULL,
    [OldValues] nvarchar(max) NULL,
    [NewValues] nvarchar(max) NULL,
    CONSTRAINT [PK_AuditEntries] PRIMARY KEY ([Id])
)
GO;

CREATE INDEX [IX_AuditEntries_Action] ON [AuditEntries] ([Action]);
GO


CREATE INDEX [IX_AuditEntries_EntityName] ON [AuditEntries] ([EntityName]);
GO
