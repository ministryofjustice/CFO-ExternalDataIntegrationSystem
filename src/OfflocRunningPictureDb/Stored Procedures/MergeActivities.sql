-- =============================================
-- Author:		Paul Cooper
-- Create date: 08/01/2023
-- Description:	Merge Offloc Staging Activities database with Offloc Running Picture
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeActivities]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY

        MERGE [OfflocRunningPicture].[Activities] AS target
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [Activity],
                   [Location],
                   [StartHour],
                   [StartMin],
                   [EndHour],
                   [EndMin]
            FROM [OfflocStagingDb].[OfflocStaging].[Activities]
        ) AS source
        ([NOMSnumber], [Activity], [Location], [StartHour], [StartMin], [EndHour], [EndMin])
        ON (
            target.[NOMSnumber] = source.[NOMSnumber] AND
            target.[Activity] = source.[Activity] AND
            target.[Location] = source.[Location] AND
            target.[StartHour] = source.[StartHour] AND
            target.[StartMin] = source.[StartMin] AND
            target.[EndHour] = source.[EndHour] AND
            target.[EndMin] = source.[EndMin]
        )
        WHEN MATCHED AND (IsActive = 0) THEN
            UPDATE SET IsActive = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [Activity],
                [Location],
                [StartHour],
                [StartMin],
                [EndHour],
                [EndMin],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[Activity], source.[Location], 
            source.[StartHour], source.[StartMin], source.[EndHour], source.[EndMin], 1)
            
        WHEN NOT MATCHED BY SOURCE AND IsActive = 1 THEN 
            UPDATE SET target.[IsActive] = 0;

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