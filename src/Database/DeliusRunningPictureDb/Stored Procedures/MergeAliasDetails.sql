-- =============================================
-- Author:		Paul Cooper
-- Create date: 30/01/2023
-- Description:	Merge Delius Staging Alias Details table with Delius Running Picture Alias Details table
-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeAliasDetails]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[AliasDetails] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [FirstName],
                   [SecondName],
                   [ThirdName],
                   [Surname],
                   [DateOfBirth],
                   [GenderCode],
                   [GenderDescription],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[AliasDetails]
        ) AS SOURCE
        ([OffenderId], [Id], [FirstName], [SecondName], [ThirdName], [Surname], [DateOfBirth], [GenderCode], [GenderDescription], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND ISNULL(
                                               NULLIF(TARGET.[FirstName], SOURCE.[FirstName]),
                                               NULLIF(SOURCE.[FirstName], TARGET.[FirstName])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[SecondName], SOURCE.[SecondName]),
                                               NULLIF(SOURCE.[SecondName], TARGET.[SecondName])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[ThirdName], SOURCE.[ThirdName]),
                                               NULLIF(SOURCE.[ThirdName], TARGET.[ThirdName])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Surname], SOURCE.[Surname]),
                                               NULLIF(SOURCE.[Surname], TARGET.[Surname])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[DateOfBirth], SOURCE.[DateOfBirth]),
                                               NULLIF(SOURCE.[DateOfBirth], TARGET.[DateOfBirth])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[GenderCode], SOURCE.[GenderCode]),
                                               NULLIF(SOURCE.[GenderCode], TARGET.[GenderCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[GenderDescription], SOURCE.[GenderDescription]),
                                               NULLIF(SOURCE.[GenderDescription], TARGET.[GenderDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OffenderId] = SOURCE.[OffenderId],
                       [FirstName] = SOURCE.[FirstName],
                       [SecondName] = SOURCE.[SecondName],
                       [ThirdName] = SOURCE.[ThirdName],
                       [Surname] = SOURCE.[Surname],
                       [DateOfBirth] = SOURCE.[DateOfBirth],
                       [GenderCode] = SOURCE.[GenderCode],
                       [GenderDescription] = SOURCE.[GenderDescription],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [FirstName],
                [SecondName],
                [ThirdName],
                [Surname],
                [DateOfBirth],
                [GenderCode],
                [GenderDescription],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[FirstName], source.[SecondName], source.[ThirdName],
             source.[Surname], source.[DateOfBirth], source.[GenderCode], source.[GenderDescription], source.[Deleted]);
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