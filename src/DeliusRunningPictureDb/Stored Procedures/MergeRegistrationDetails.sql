-- =============================================
-- Author:		Paul Cooper
-- Create date: 31/01/2023
-- Description:	Merge Delius Staging Registration Details table with Delius Running Picture Registration Details table
-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeRegistrationDetails]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[RegistrationDetails] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [Date],
                   [TypeCode],
                   [TypeDescription],
                   [CategoryCode],
                   [CategoryDescription],
                   [RegisterCode],
                   [RegisterDescription],
                   [DeRegistered],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[RegistrationDetails]
        ) AS SOURCE
        ([OffenderId], [Id], [Date], [TypeCode], [TypeDescription], [CategoryCode], [CategoryDescription], [RegisterCode], [RegisterDescription], [DeRegistered], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND ISNULL(NULLIF(TARGET.[Date], SOURCE.[Date]), NULLIF(SOURCE.[Date], TARGET.[Date])) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[TypeCode], SOURCE.[TypeCode]),
                                               NULLIF(SOURCE.[TypeCode], TARGET.[TypeCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[TypeDescription], SOURCE.[TypeDescription]),
                                               NULLIF(SOURCE.[TypeDescription], TARGET.[TypeDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[CategoryCode], SOURCE.[CategoryCode]),
                                               NULLIF(SOURCE.[CategoryCode], TARGET.[CategoryCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[CategoryDescription], SOURCE.[CategoryDescription]),
                                               NULLIF(SOURCE.[CategoryDescription], TARGET.[CategoryDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[RegisterCode], SOURCE.[RegisterCode]),
                                               NULLIF(SOURCE.[RegisterCode], TARGET.[RegisterCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[RegisterDescription], SOURCE.[RegisterDescription]),
                                               NULLIF(SOURCE.[RegisterDescription], TARGET.[RegisterDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[DeRegistered], SOURCE.[DeRegistered]),
                                               NULLIF(SOURCE.[DeRegistered], TARGET.[DeRegistered])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OffenderId] = SOURCE.[OffenderId],
                       [Date] = SOURCE.[Date],
                       [TypeCode] = SOURCE.[TypeCode],
                       [TypeDescription] = SOURCE.[TypeDescription],
                       [CategoryCode] = SOURCE.[CategoryCode],
                       [CategoryDescription] = SOURCE.[CategoryDescription],
                       [RegisterCode] = SOURCE.[RegisterCode],
                       [RegisterDescription] = SOURCE.[RegisterDescription],
                       [DeRegistered] = SOURCE.[DeRegistered],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [Date],
                [TypeCode],
                [TypeDescription],
                [CategoryCode],
                [CategoryDescription],
                [RegisterCode],
                [RegisterDescription],
                [DeRegistered],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[Date], source.[TypeCode], source.[TypeDescription],
             source.[CategoryCode], source.[CategoryDescription], source.[RegisterCode], source.[RegisterDescription],
             source.[DeRegistered], source.[Deleted]);
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