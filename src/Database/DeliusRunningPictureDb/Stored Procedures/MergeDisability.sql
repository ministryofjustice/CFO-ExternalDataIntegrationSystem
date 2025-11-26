-- =============================================
-- Author:		Paul Cooper 
-- Create date: 30/01/2023
-- Description:	Merge Delius Staging Disability table with Delius Running Picture Disability table
-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeDisability]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[Disability] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [TypeCode],
                   [TypeDescription],
                   [StartDate],
                   [EndDate],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[Disability]
        ) AS SOURCE
        ([OffenderId], [Id], [TypeCode], [TypeDescription], [StartDate], [EndDate], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND ISNULL(
                                               NULLIF(TARGET.[TypeCode], SOURCE.[TypeCode]),
                                               NULLIF(SOURCE.[TypeCode], TARGET.[TypeCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[TypeDescription], SOURCE.[TypeDescription]),
                                               NULLIF(SOURCE.[TypeDescription], TARGET.[TypeDescription])
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
                       [TypeCode] = SOURCE.[TypeCode],
                       [TypeDescription] = SOURCE.[TypeDescription],
                       [StartDate] = SOURCE.[StartDate],
                       [EndDate] = SOURCE.[EndDate],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [TypeCode],
                [TypeDescription],
                [StartDate],
                [EndDate],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[TypeCode], source.[TypeDescription], source.[StartDate],
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