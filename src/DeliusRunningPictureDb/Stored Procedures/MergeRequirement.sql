-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeRequirement]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[Requirement] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [DisposalId],
                   [StartDate],
                   [CommencementDate],
                   [Terminationdate],
                   [TerminationReasonCode],
                   [TerminationDescription],
                   [CategoryCode],
                   [CategoryDescription],
                   [SubCategoryCode],
                   [SubCategoryDescription],
                   [Length],
                   [UnitCode],
                   [UnitDescription],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[Requirement]
        ) AS SOURCE
        ([OffenderId], [Id], [DisposalId], [StartDate], [CommencementDate], [Terminationdate], [TerminationReasonCode], [TerminationDescription], [CategoryCode], [CategoryDescription], [SubCategoryCode], [SubCategoryDescription], [Length], [UnitCode], [UnitDescription], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND TARGET.[DisposalId] = SOURCE.[DisposalId]
                                 AND ISNULL(
                                               NULLIF(TARGET.[StartDate], SOURCE.[StartDate]),
                                               NULLIF(SOURCE.[StartDate], TARGET.[StartDate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[CommencementDate], SOURCE.[CommencementDate]),
                                               NULLIF(SOURCE.[CommencementDate], TARGET.[CommencementDate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Terminationdate], SOURCE.[Terminationdate]),
                                               NULLIF(SOURCE.[Terminationdate], TARGET.[Terminationdate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[TerminationReasonCode], SOURCE.[TerminationReasonCode]),
                                               NULLIF(SOURCE.[TerminationReasonCode], TARGET.[TerminationReasonCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[TerminationDescription], SOURCE.[TerminationDescription]),
                                               NULLIF(SOURCE.[TerminationDescription], TARGET.[TerminationDescription])
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
                                               NULLIF(TARGET.[SubCategoryCode], SOURCE.[SubCategoryCode]),
                                               NULLIF(SOURCE.[SubCategoryCode], TARGET.[SubCategoryCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[SubCategoryDescription], SOURCE.[SubCategoryDescription]),
                                               NULLIF(SOURCE.[SubCategoryDescription], TARGET.[SubCategoryDescription])
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
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OffenderId] = SOURCE.[OffenderId],
                       [DisposalId] = SOURCE.[DisposalId],
                       [StartDate] = SOURCE.[StartDate],
                       [CommencementDate] = SOURCE.[CommencementDate],
                       [Terminationdate] = SOURCE.[Terminationdate],
                       [TerminationReasonCode] = SOURCE.[TerminationReasonCode],
                       [TerminationDescription] = SOURCE.[TerminationDescription],
                       [CategoryCode] = SOURCE.[CategoryCode],
                       [CategoryDescription] = SOURCE.[CategoryDescription],
                       [SubCategoryCode] = SOURCE.[SubCategoryCode],
                       [SubCategoryDescription] = SOURCE.[SubCategoryDescription],
                       [Length] = SOURCE.[Length],
                       [UnitCode] = SOURCE.[UnitCode],
                       [UnitDescription] = SOURCE.[UnitDescription],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [DisposalId],
                [StartDate],
                [CommencementDate],
                [Terminationdate],
                [TerminationReasonCode],
                [TerminationDescription],
                [CategoryCode],
                [CategoryDescription],
                [SubCategoryCode],
                [SubCategoryDescription],
                [Length],
                [UnitCode],
                [UnitDescription],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[DisposalId], source.[StartDate], source.[CommencementDate],
             source.[Terminationdate], source.[TerminationReasonCode], source.[TerminationDescription],
             source.[CategoryCode], source.[CategoryDescription], source.[SubCategoryCode],
             source.[SubCategoryDescription], source.[Length], source.[UnitCode], source.[UnitDescription],
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