CREATE PROCEDURE [DeliusStaging].[StageDelius]
	@processedFile VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	SET DATEFORMAT DMY;

	BEGIN TRANSACTION;
	BEGIN TRY
		DECLARE @retMessage VARCHAR(500);
		EXEC [DeliusStaging].[StandardiseData] @retMessage OUTPUT;

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
END
