---- =============================================
---- Author:		Paul Cooper & Dan Fenelon
---- Create date: 29/01/2023
---- Description:	Merge Delius Staging Additional Identifier table with Delius Running Picture Additional Identifier table
---- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
---- =============================================
CREATE PROCEDURE [DeliusRunningPicture].[MergeAdditionalIdentifier]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[AdditionalIdentifier] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [PNC],
                   [YOT],
                   [OldPnc],
                   [MilitaryServiceNumber],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[AdditionalIdentifier]
        ) AS SOURCE
        ([OffenderId], [Id], [PNC], [YOT], [OldPnc], [MilitaryServiceNumber], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND ISNULL(NULLIF(TARGET.[PNC], SOURCE.[PNC]), NULLIF(SOURCE.[PNC], TARGET.[PNC])) IS NULL
                                 AND ISNULL(NULLIF(TARGET.[YOT], SOURCE.[YOT]), NULLIF(SOURCE.[YOT], TARGET.[YOT])) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[OldPnc], SOURCE.[OldPnc]),
                                               NULLIF(SOURCE.[OldPnc], TARGET.[OldPnc])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[MilitaryServiceNumber], SOURCE.[MilitaryServiceNumber]),
                                               NULLIF(SOURCE.[MilitaryServiceNumber], TARGET.[MilitaryServiceNumber])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OffenderId] = SOURCE.[OffenderId],
                       [PNC] = SOURCE.[PNC],
                       [YOT] = SOURCE.[YOT],
                       [OldPnc] = SOURCE.[OldPnc],
                       [MilitaryServiceNumber] = SOURCE.[MilitaryServiceNumber],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [PNC],
                [YOT],
                [OldPnc],
                [MilitaryServiceNumber],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[PNC], source.[YOT], source.[OldPnc],
             source.[MilitaryServiceNumber], source.[Deleted]);
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