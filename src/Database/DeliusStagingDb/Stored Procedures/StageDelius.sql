CREATE PROCEDURE [DeliusStaging].[StageDelius]
	@processedFile VARCHAR(50)
AS
	SET NOCOUNT ON;
	SET DATEFORMAT DMY;

	BEGIN TRANSACTION;
	BEGIN TRY
		DECLARE @retMessage varchar(500);
		EXEC [DeliusStaging].[StandardiseData] @retMessage;

		UPDATE [DeliusRunningPictureDb].[DeliusRunningPicture].[ProcessedFiles]
		SET [Status] = 'Imported'
		WHERE FileName = @processedFile;

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH
RETURN 0
