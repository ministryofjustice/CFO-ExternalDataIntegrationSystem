-- =============================================
-- Author:		Paul Cooper 
-- Create date: 31/01/2023
-- Description:	Merge Delius Staging Provision table with Delius Running Picture Provision table
-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeProvision]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[Provision] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [Code],
                   [Description],
                   [StartDate],
                   [EndDate],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[Provision]
        ) AS SOURCE
        ([OffenderId], [Id], [Code], [Description], [StartDate], [EndDate], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND ISNULL(NULLIF(TARGET.[Code], SOURCE.[Code]), NULLIF(SOURCE.[Code], TARGET.[Code])) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Description], SOURCE.[Description]),
                                               NULLIF(SOURCE.[Description], TARGET.[Description])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[StartDate], SOURCE.[StartDate]),
                                               NULLIF(SOURCE.[StartDate], TARGET.[StartDate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[EndDate], SOURCE.[EndDate]),
                                               NULLIF(SOURCE.[EndDate], TARGET.[EndDate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OffenderId] = SOURCE.[OffenderId],
                       [Code] = SOURCE.[Code],
                       [Description] = SOURCE.[Description],
                       [StartDate] = SOURCE.[StartDate],
                       [EndDate] = SOURCE.[EndDate],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [Code],
                [Description],
                [StartDate],
                [EndDate],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[Code], source.[Description], source.[StartDate],
             source.[EndDate], source.[Deleted]);
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