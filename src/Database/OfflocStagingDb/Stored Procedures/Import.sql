-- =============================================
-- Author:		Daniel Fenelon
-- Create date: 18/03/2024
-- Description:	Import of the OffLoc files
-- Note:        File loading removed - data is now staged via SqlBulkCopy
--              in the DbInteractions service. This procedure handles
--              post-load standardisation and status update only.
-- =============================================

CREATE PROCEDURE [OfflocStaging].[Import]
    @processedFile VARCHAR(50)
AS
BEGIN

    SET NOCOUNT ON;
    SET DATEFORMAT DMY;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @retMessage VARCHAR(500);
        EXEC [OfflocStaging].[StandardiseData] @retMessage OUTPUT;

        UPDATE [OfflocRunningPictureDb].[OfflocRunningPicture].[ProcessedFiles]
        SET [Status] = 'Imported'
        WHERE FileName = @processedFile;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;