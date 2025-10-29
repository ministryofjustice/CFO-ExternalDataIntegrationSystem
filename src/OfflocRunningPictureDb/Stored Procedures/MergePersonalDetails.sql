-- =============================================
-- Author:		Paul Cooper
-- Create date: 18/01/2023
-- Description:	Merge Offloc Staging Personal Details table with Offloc Running Picture Personal Details table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergePersonalDetails]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY

        MERGE [OfflocRunningPicture].[PersonalDetails] AS TARGET
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [Forename1],
                   [Forename2],
                   [Surname],
                   [DOB],
                   [Gender],
                   [MaternityStatus],
                   [Nationality],
                   [Religion],
                   [MaritalStatus],
                   [EthnicGroup]
            FROM [OfflocStagingDb].[OfflocStaging].[PersonalDetails]
        ) AS SOURCE
        ([NOMSnumber], [Forename1], [Forename2], [Surname], [DOB], [Gender], [MaternityStatus], [Nationality], [Religion], [MaritalStatus], [EthnicGroup])
        ON (TARGET.[NOMSnumber] = SOURCE.[NOMSnumber])
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.[Forename1], SOURCE.[Forename1]),
                                           NULLIF(SOURCE.[Forename1], TARGET.[Forename1])
                                       ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Forename2], SOURCE.[Forename2]),
                                               NULLIF(SOURCE.[Forename2], TARGET.[Forename2])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Surname], SOURCE.[Surname]),
                                               NULLIF(SOURCE.[Surname], TARGET.[Surname])
                                           ) IS NULL
                                 AND TARGET.[DOB] = SOURCE.[DOB]
                                 AND TARGET.[Gender] = SOURCE.[Gender]
                                 AND ISNULL(
                                               NULLIF(TARGET.[MaternityStatus], SOURCE.[MaternityStatus]),
                                               NULLIF(SOURCE.[MaternityStatus], TARGET.[MaternityStatus])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Nationality], SOURCE.[Nationality]),
                                               NULLIF(SOURCE.[Nationality], TARGET.[Nationality])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Religion], SOURCE.[Religion]),
                                               NULLIF(SOURCE.[Religion], TARGET.[Religion])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[MaritalStatus], SOURCE.[MaritalStatus]),
                                               NULLIF(SOURCE.[MaritalStatus], TARGET.[MaritalStatus])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[EthnicGroup], SOURCE.[EthnicGroup]),
                                               NULLIF(SOURCE.[EthnicGroup], TARGET.[EthnicGroup])
                                           ) IS NULL
                                 AND TARGET.[IsActive] = 1
                             ) THEN
            UPDATE SET [Forename1] = SOURCE.[Forename1],
                       [Forename2] = SOURCE.[Forename2],
                       [Surname] = SOURCE.[Surname],
                       [DOB] = SOURCE.[DOB],
                       [Gender] = SOURCE.[Gender],
                       [MaternityStatus] = SOURCE.[MaternityStatus],
                       [Nationality] = SOURCE.[Nationality],
                       [Religion] = SOURCE.[Religion],
                       [MaritalStatus] = SOURCE.[MaritalStatus],
                       [EthnicGroup] = SOURCE.[EthnicGroup],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [Forename1],
                [Forename2],
                [Surname],
                [DOB],
                [Gender],
                [MaternityStatus],
                [Nationality],
                [Religion],
                [MaritalStatus],
                [EthnicGroup],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[Forename1], source.[Forename2], source.[Surname], source.[DOB],
             source.[Gender], source.[MaternityStatus], source.[Nationality], source.[Religion],
             source.[MaritalStatus], source.[EthnicGroup], 1)
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