CREATE PROCEDURE [DeliusStaging].[StandardiseNationality] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  

		;WITH cte AS(
			SELECT 
				[NationalityDescription], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseNationality]([NationalityDescription]) AS [CleanNationalityDescription] 
			FROM
				[DeliusStagingDb].[DeliusStaging].[Offenders] 
			WHERE 
				[NationalityDescription] IS NOT NULL
		)
		UPDATE cte 
			SET cte.[NationalityDescription] = cte.[CleanNationalityDescription]
		WHERE 
			cte.[NationalityDescription] <> cte.[CleanNationalityDescription]
			AND cte.[CleanNationalityDescription] IS NOT NULL

		;WITH cte AS(
			SELECT 
				[SecondNationalityDescription], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseNationality]([SecondNationalityDescription]) AS [CleanSecondNationalityDescription] 
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE [SecondNationalityDescription] IS NOT NULL
		)
		UPDATE cte 
			SET cte.[SecondNationalityDescription] = cte.[CleanSecondNationalityDescription]
		WHERE 
			cte.[SecondNationalityDescription] <> cte.[CleanSecondNationalityDescription] 	
			AND cte.[CleanSecondNationalityDescription] IS NOT NULL

		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END