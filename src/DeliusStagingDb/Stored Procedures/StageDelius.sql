CREATE PROCEDURE [DeliusStaging].[StageDelius]
	@basePath VARCHAR(100), -- Should be in the format /app/delius/{fileName}.
	@processedFile VARCHAR(50),
	@inContainer VARCHAR(1)
AS
	IF @inContainer = 'N'
	BEGIN 
		SET NOCOUNT ON; 
		SET DATEFORMAT DMY;
	END;

	CREATE TABLE #TableNames(Id int Primary Key Identity, fileName VARCHAR(50));
	INSERT INTO #TableNames(fileName) VALUES
		('AdditionalIdentifier'), ('AliasDetails'), ('Disability'), ('Disposal'), ('EventDetails'), ('Header'), ('MainOffence'), 
		('OAS'), ('OffenderAddress'), ('OffenderManager'), ('OffenderManagerBuildings'), ('OffenderManagerTeam'), 
		('OffenderToOffenderManagerMappings'), ('Offenders'), ('OffenderTransfer'), ('PersonalCircumstances'), 
		('Provision'), ('RegistrationDetails'), ('Requirement');
	
	DECLARE @i int;
	DECLARE @sql NVARCHAR(250);
	DECLARE @fileName VARCHAR(150);

	SET @i = 1;

	BEGIN TRANSACTION;	
	BEGIN TRY
		WHILE @i <= (Select COUNT(*) FROM #TableNames)
		BEGIN
			SET @fileName = (SELECT fileName FROM #TableNames WHERE Id = @i);
			SET @sql = 'BULK INSERT DeliusStaging.' + @fileName + ' FROM ''' + @basePath + @fileName + '.txt''' +
			' WITH (FieldTerminator=''|'', RowTerminator = ''0x0d0a'', MAXERRORS = 1000)';
			
			EXEC (@sql);
			SET @i = @i+1;
		END

		--Standardise Delius Data
        Declare @retMessage varchar(500);
		EXEC [DeliusStaging].[StandardiseData] @retMessage;

		INSERT INTO [DeliusRunningPictureDb].[DeliusRunningPicture].[ProcessedFiles] (FileName, FileId) VALUES (@processedFile, CAST(SUBSTRING(@processedFile, 12, 4) AS int));
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH
RETURN 0
