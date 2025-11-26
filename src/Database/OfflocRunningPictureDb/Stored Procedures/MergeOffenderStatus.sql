-- =============================================
-- Author:		Paul Cooper
-- Create date: 18/01/2023
-- Description:	Merge Offloc Staging Offender Status table with Offloc Running Picture Offender Status table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeOffenderStatus]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[OffenderStatus] AS TARGET
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [CustodyStatus],
                   [InmateStatus]
            FROM [OfflocStagingDb].[OfflocStaging].[OffenderStatus]
        ) AS SOURCE
        ([NOMSnumber], [CustodyStatus], [InmateStatus])
        ON (TARGET.[NOMSnumber] = SOURCE.[NOMSnumber])
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.[CustodyStatus], SOURCE.[CustodyStatus]),
                                           NULLIF(SOURCE.[CustodyStatus], TARGET.[CustodyStatus])
                                       ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[InmateStatus], SOURCE.[InmateStatus]),
                                               NULLIF(SOURCE.[InmateStatus], TARGET.[InmateStatus])
                                           ) IS NULL
                                 AND TARGET.[IsActive] = 1
                             ) THEN
            UPDATE SET [CustodyStatus] = SOURCE.[CustodyStatus],
                       [InmateStatus] = SOURCE.[InmateStatus],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [CustodyStatus],
                [InmateStatus],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[CustodyStatus], source.[InmateStatus], 1)
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