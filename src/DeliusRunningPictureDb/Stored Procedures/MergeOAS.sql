-- =============================================
-- Author:		Paul Cooper 
-- Create date: 30/01/2023
-- Description:	Merge Delius Staging OAS table with Delius Running Picture OAS table
-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeOAS]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[OAS] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [ROSHScore],
                   [AssesmentDate],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[OAS]
        ) AS SOURCE
        ([OffenderId], [Id], [ROSHScore], [AssesmentDate], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND ISNULL(
                                               NULLIF(TARGET.[ROSHScore], SOURCE.[ROSHScore]),
                                               NULLIF(SOURCE.[ROSHScore], TARGET.[ROSHScore])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[AssesmentDate], SOURCE.[AssesmentDate]),
                                               NULLIF(SOURCE.[AssesmentDate], TARGET.[AssesmentDate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OffenderId] = SOURCE.[OffenderId],
                       [ROSHScore] = SOURCE.[ROSHScore],
                       [AssesmentDate] = SOURCE.[AssesmentDate],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [ROSHScore],
                [AssesmentDate],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[ROSHScore], source.[AssesmentDate], source.[Deleted]);
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