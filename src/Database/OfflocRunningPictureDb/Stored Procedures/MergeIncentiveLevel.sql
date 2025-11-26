-- =============================================
-- Author:		Paul Cooper
-- Create date: 17/01/2023
-- Description:	Merge Offloc Staging IncentiveLevel table with Offloc Running Picture IncentiveLevel table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeIncentiveLevel]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[IncentiveLevel] AS TARGET
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [IncentiveBand]
            FROM [OfflocStagingDb].[OfflocStaging].[IncentiveLevel]
        ) AS SOURCE
        ([NOMSnumber], [IncentiveBand])
        ON (TARGET.[NOMSnumber] = SOURCE.[NOMSnumber])
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.[IncentiveBand], SOURCE.[IncentiveBand]),
                                           NULLIF(SOURCE.[IncentiveBand], TARGET.[IncentiveBand])
                                       ) IS NULL
                                 AND TARGET.[IsActive] = 1
                             ) THEN
            UPDATE SET [IncentiveBand] = SOURCE.[IncentiveBand],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [IncentiveBand],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[IncentiveBand], 1)
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