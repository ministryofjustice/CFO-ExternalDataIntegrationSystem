CREATE PROCEDURE [OfflocRunningPicture].[MergePreviousPrisonNumbers]
AS
BEGIN;
	SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[PreviousPrisonNumbers] AS target
	        USING (SELECT DISTINCT [NOMSnumber], [Details] FROM [OfflocStagingDb].[OfflocStaging].[PreviousPrisonNumbers])
            AS source ([NOMSnumber], [Details])
	    ON (
            target.[NOMSnumber] = source.[NOMSnumber] AND
            target.[Details] = source.[Details]
        )
        WHEN MATCHED AND NOT (IsActive = 1) THEN 
            UPDATE SET IsActive = 1
	    WHEN NOT MATCHED THEN
	       INSERT ([NOMSnumber], [Details], [IsActive])
	       VALUES (source.[NOMSnumber], source.[Details], 1)
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
