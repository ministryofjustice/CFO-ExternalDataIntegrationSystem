-- =============================================
-- Author:		Paul Cooper 
-- Create date: 3/01/2023
-- Description:	Merge Delius Staging Main Offence Table with Delius Running Picture Main Offence table
-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeMainOffence]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[MainOffence] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [EventId],
                   [OffenceCode],
                   [OffenceDescription],
                   [OffenceDate],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[MainOffence]
        ) AS SOURCE
        ([OffenderId], [Id], [EventId], [OffenceCode], [OffenceDescription], [OffenceDate], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND TARGET.[EventId] = SOURCE.[EventId]
                                 AND ISNULL(
                                               NULLIF(TARGET.[OffenceCode], SOURCE.[OffenceCode]),
                                               NULLIF(SOURCE.[OffenceCode], TARGET.[OffenceCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[OffenceDescription], SOURCE.[OffenceDescription]),
                                               NULLIF(SOURCE.[OffenceDescription], TARGET.[OffenceDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[OffenceDate], SOURCE.[OffenceDate]),
                                               NULLIF(SOURCE.[OffenceDate], TARGET.[OffenceDate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OffenderId] = SOURCE.[OffenderId],
                       [EventId] = SOURCE.[EventId],
                       [OffenceCode] = SOURCE.[OffenceCode],
                       [OffenceDescription] = SOURCE.[OffenceDescription],
                       [OffenceDate] = SOURCE.[OffenceDate],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [EventId],
                [OffenceCode],
                [OffenceDescription],
                [OffenceDate],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[EventId], source.[OffenceCode], source.[OffenceDescription],
             source.[OffenceDate], source.[Deleted]);
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