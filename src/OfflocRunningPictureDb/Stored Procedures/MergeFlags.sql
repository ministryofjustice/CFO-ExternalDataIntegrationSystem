-- =============================================
-- Author:		Paul Cooper
-- Create date: 17/01/2023
-- Description:	Merge Offloc Staging Flags table with Offloc Running Picture Flags table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeFlags]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[Flags] AS target
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [Details]
            FROM [OfflocStagingDb].[OfflocStaging].[Flags]
        ) AS source
        ([NOMSnumber], [Details])
        ON (
            target.[NOMSnumber] = source.[NOMSnumber] AND
            target.[Details] = source.[Details]    
        )
        WHEN MATCHED AND NOT (target.[IsActive] = 1) THEN 
            UPDATE SET IsActive = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [Details],
                [IsActive]
            )
            VALUES(source.[NOMSnumber], source.[Details], 1)
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