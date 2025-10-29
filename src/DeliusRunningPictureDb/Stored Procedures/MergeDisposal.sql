-- =============================================
-- Author:		Paul Cooper 
-- Create date: 30/01/2024
-- Description:	Merge Delius Staging Disposal table with Delius Running Picture Disposal table
-- =============================================
-- Author:		Paul Cooper 
-- Create date: 08/05/2024
-- Description:	Altered the driver column from eventid to id.
-- =============================================
CREATE PROCEDURE [DeliusRunningPicture].[MergeDisposal]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[Disposal] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [EventId],
                   [SentenceDate],
                   [Length],
                   [UnitCode],
                   [UnitDescription],
                   [DisposalCode],
                   [DisposalDetail],
                   [DisposalTerminationCode],
                   [DisposalTerminationDescription],
                   [TerminationDate],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[Disposal]
        ) AS SOURCE
        ([OffenderId], [Id], [EventId], [SentenceDate], [Length], [UnitCode], [UnitDescription], [DisposalCode], [DisposalDetail], [DisposalTerminationCode], [DisposalTerminationDescription], [TerminationDate], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND ISNULL(
                                               NULLIF(TARGET.[EventId], SOURCE.[EventId]),
                                               NULLIF(SOURCE.[EventId], TARGET.[EventId])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[SentenceDate], SOURCE.[SentenceDate]),
                                               NULLIF(SOURCE.[SentenceDate], TARGET.[SentenceDate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Length], SOURCE.[Length]),
                                               NULLIF(SOURCE.[Length], TARGET.[Length])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[UnitCode], SOURCE.[UnitCode]),
                                               NULLIF(SOURCE.[UnitCode], TARGET.[UnitCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[UnitDescription], SOURCE.[UnitDescription]),
                                               NULLIF(SOURCE.[UnitDescription], TARGET.[UnitDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[DisposalCode], SOURCE.[DisposalCode]),
                                               NULLIF(SOURCE.[DisposalCode], TARGET.[DisposalCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[DisposalDetail], SOURCE.[DisposalDetail]),
                                               NULLIF(SOURCE.[DisposalDetail], TARGET.[DisposalDetail])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(
                                                         TARGET.[DisposalTerminationCode],
                                                         SOURCE.[DisposalTerminationCode]
                                                     ),
                                               NULLIF(
                                                         SOURCE.[DisposalTerminationCode],
                                                         TARGET.[DisposalTerminationCode]
                                                     )
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(
                                                         TARGET.DisposalTerminationDescription,
                                                         SOURCE.DisposalTerminationDescription
                                                     ),
                                               NULLIF(
                                                         SOURCE.DisposalTerminationDescription,
                                                         TARGET.DisposalTerminationDescription
                                                     )
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.TerminationDate, SOURCE.TerminationDate),
                                               NULLIF(SOURCE.TerminationDate, TARGET.TerminationDate)
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OffenderId] = SOURCE.[OffenderId],
                       [EventId] = SOURCE.[EventId],
                       [SentenceDate] = SOURCE.[SentenceDate],
                       [Length] = SOURCE.[Length],
                       [UnitCode] = SOURCE.[UnitCode],
                       [UnitDescription] = SOURCE.[UnitDescription],
                       [DisposalCode] = SOURCE.[DisposalCode],
                       [DisposalDetail] = SOURCE.[DisposalDetail],
                       [DisposalTerminationCode] = SOURCE.[DisposalTerminationCode],
                       [DisposalTerminationDescription] = SOURCE.[DisposalTerminationDescription],
                       [TerminationDate] = SOURCE.[TerminationDate],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [EventId],
                [SentenceDate],
                [Length],
                [UnitCode],
                [UnitDescription],
                [DisposalCode],
                [DisposalDetail],
                [DisposalTerminationCode],
                [DisposalTerminationDescription],
                [TerminationDate],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[EventId], source.[SentenceDate], source.[Length],
             source.[UnitCode], source.[UnitDescription], source.[DisposalCode], source.[DisposalDetail],
             source.[DisposalTerminationCode], source.[DisposalTerminationDescription], source.[TerminationDate],
             source.[Deleted]);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;
        ; THROW;
    END CATCH;
END;