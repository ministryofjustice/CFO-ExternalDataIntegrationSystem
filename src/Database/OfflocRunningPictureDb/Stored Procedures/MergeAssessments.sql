-- =============================================
-- Author:		Paul Cooper
-- Create date: 10/01/2023
-- Description:	Merge Offloc Staging Assessments table with Offloc Running Picture Assessments table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeAssessments]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[Assessments] AS TARGET
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [SecurityCategory],
                   [DateSecurityCategoryReview],
                   [DateSecCatChanged]
            FROM [OfflocStagingDb].[OfflocStaging].[Assessments]
        ) AS SOURCE
        ([NOMSnumber], [SecurityCategory], [DateSecurityCategoryReview], [DateSecCatChanged])
        ON (TARGET.[NOMSnumber] = SOURCE.[NOMSnumber])
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.[SecurityCategory], SOURCE.[SecurityCategory]),
                                           NULLIF(SOURCE.[SecurityCategory], TARGET.[SecurityCategory])
                                       ) IS NULL
                                 AND ISNULL(
                                               NULLIF(
                                                         TARGET.[DateSecurityCategoryReview],
                                                         SOURCE.[DateSecurityCategoryReview]
                                                     ),
                                               NULLIF(
                                                         SOURCE.[DateSecurityCategoryReview],
                                                         TARGET.[DateSecurityCategoryReview]
                                                     )
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[DateSecCatChanged], SOURCE.[DateSecCatChanged]),
                                               NULLIF(SOURCE.[DateSecCatChanged], TARGET.[DateSecCatChanged])
                                           ) IS NULL
                                 AND TARGET.[IsActive] = 1
                             ) THEN
            UPDATE SET [SecurityCategory] = SOURCE.[SecurityCategory],
                       [DateSecurityCategoryReview] = SOURCE.[DateSecurityCategoryReview],
                       [DateSecCatChanged] = SOURCE.[DateSecCatChanged],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [SecurityCategory],
                [DateSecurityCategoryReview],
                [DateSecCatChanged],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[SecurityCategory], source.[DateSecurityCategoryReview],
             source.[DateSecCatChanged], 1)
        WHEN NOT MATCHED BY SOURCE AND IsActive = 1 THEN
            UPDATE SET IsActive = 0;
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