-- =============================================
-- Author:		Paul Cooper 
-- Create date: 30/01/2023
-- Description:	Merge Delius Staging Personal Circumstances table with Delius Running Picture Personal Circumstances table
-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergePersonalCircumstances]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[PersonalCircumstances] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [Type],
                   [TypeDescription],
                   [SubType],
                   [SubTypeDescription],
                   [StartDate],
                   [EndDate],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[PersonalCircumstances]
        ) AS SOURCE
        ([OffenderId], [Id], [Type], [TypeDescription], [SubType], [SubTypeDescription], [StartDate], [EndDate], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND ISNULL(NULLIF(TARGET.[Type], SOURCE.[Type]), NULLIF(SOURCE.[Type], TARGET.[Type])) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[TypeDescription], SOURCE.[TypeDescription]),
                                               NULLIF(SOURCE.[TypeDescription], TARGET.[TypeDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[SubType], SOURCE.[SubType]),
                                               NULLIF(SOURCE.[SubType], TARGET.[SubType])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[SubTypeDescription], SOURCE.[SubTypeDescription]),
                                               NULLIF(SOURCE.[SubTypeDescription], TARGET.[SubTypeDescription])
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
                       [Type] = SOURCE.[Type],
                       [TypeDescription] = SOURCE.[TypeDescription],
                       [SubType] = SOURCE.[SubType],
                       [SubTypeDescription] = SOURCE.[SubTypeDescription],
                       [StartDate] = SOURCE.[StartDate],
                       [EndDate] = SOURCE.[EndDate],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [Type],
                [TypeDescription],
                [SubType],
                [SubTypeDescription],
                [StartDate],
                [EndDate],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[Type], source.[TypeDescription], source.[SubType],
             source.[SubTypeDescription], source.[StartDate], source.[EndDate], source.[Deleted]);
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