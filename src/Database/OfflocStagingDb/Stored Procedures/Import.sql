-- =============================================
-- Author:		Daniel Fenelon
-- Create date: 18/03/2024
-- Description:	Import of the OffLoc files
-- =============================================

CREATE PROCEDURE [OfflocStaging].[Import]
    @processedFile VARCHAR(50)
AS
BEGIN

    SET NOCOUNT ON;
    SET DATEFORMAT DMY;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @retMessage varchar(500);
        EXEC [OfflocStaging].[StandardiseData] @retMessage;

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