-- =============================================
-- Author:		Paul Cooper
-- Create date: 17/01/2023
-- Description:	Merge Offloc Staging Locations table with Offloc Running Picture Locations table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeLocations]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[Locations] AS TARGET
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [Location]
            FROM [OfflocStagingDb].[OfflocStaging].[Locations]
        ) AS SOURCE
        ([NOMSnumber], [Location])
        ON (TARGET.[NOMSnumber] = SOURCE.[NOMSnumber])
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.[Location], SOURCE.[Location]),
                                           NULLIF(SOURCE.[Location], TARGET.[Location])
                                       ) IS NULL
                                 AND TARGET.[IsActive] = 1
                             ) THEN
            UPDATE SET [Location] = SOURCE.[Location],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [Location],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[Location], 1)
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