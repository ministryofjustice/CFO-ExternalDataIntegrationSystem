-- =============================================
-- Author:		Paul Cooper
-- Create date: 17/01/2023
-- Description:	Merge Offloc Staging Employment table with Offloc Running Picture Employment table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeEmployment]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[Employment] AS TARGET
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [Employed],
                   [EmploymentStatusReception],
                   [EmploymentStatusDischarge]
            FROM [OfflocStagingDb].[OfflocStaging].[Employment]
        ) AS SOURCE
        ([NOMSnumber], [Employed], [EmploymentStatusReception], [EmploymentStatusDischarge])
        ON (TARGET.[NOMSnumber] = SOURCE.[NOMSnumber])
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.[Employed], SOURCE.[Employed]),
                                           NULLIF(SOURCE.[Employed], TARGET.[Employed])
                                       ) IS NULL
                                 AND ISNULL(
                                               NULLIF(
                                                         TARGET.[EmploymentStatusReception],
                                                         SOURCE.[EmploymentStatusReception]
                                                     ),
                                               NULLIF(
                                                         SOURCE.[EmploymentStatusReception],
                                                         TARGET.[EmploymentStatusReception]
                                                     )
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(
                                                         TARGET.[EmploymentStatusDischarge],
                                                         SOURCE.[EmploymentStatusDischarge]
                                                     ),
                                               NULLIF(
                                                         SOURCE.[EmploymentStatusDischarge],
                                                         TARGET.[EmploymentStatusDischarge]
                                                     )
                                           ) IS NULL
                                 AND TARGET.[IsActive] = 1
                             ) THEN
            UPDATE SET [Employed] = SOURCE.[Employed],
                       [EmploymentStatusReception] = SOURCE.[EmploymentStatusReception],
                       [EmploymentStatusDischarge] = SOURCE.[EmploymentStatusDischarge],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [Employed],
                [EmploymentStatusReception],
                [EmploymentStatusDischarge],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[Employed], source.[EmploymentStatusReception],
             source.[EmploymentStatusDischarge], 1)
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