CREATE PROCEDURE [DeliusStaging].[StandardiseGender] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  

		--Cleans GenderCode in Offenders table
		;WITH cte AS(
			SELECT 
				OffenderId, 
				GenderCode, 
				GenderDescription, 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseGender](GenderCode) AS CleanGender  
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE 
				GenderCode IS NOT NULL
		)
		UPDATE cte 
			SET GenderDescription = CleanGender
		WHERE 
			GenderDescription <> CleanGender 
			AND CleanGender IS NOT NULL

	
		--Cleans GenderCode in AliasDetails table
		;WITH cte AS(
			SELECT 
				OffenderId, 
				GenderCode, 
				GenderDescription, 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseGender](GenderCode) AS CleanGender  
			FROM 
				[DeliusStaging].[AliasDetails] 
			WHERE 
				GenderCode IS NOT NULL
		)
		UPDATE cte 
			SET GenderDescription = CleanGender
		WHERE 
			GenderDescription <> CleanGender 
			AND CleanGender IS NOT NULL



		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END