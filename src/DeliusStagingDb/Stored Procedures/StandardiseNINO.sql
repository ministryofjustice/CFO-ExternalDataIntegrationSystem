CREATE PROCEDURE [DeliusStaging].[StandardiseNINO] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  

		;WITH cte AS(
			SELECT 
				[NINO], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseNINO]([NINO]) AS [CleanNINO] 
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE [NINO] IS NOT NULL
		)
		UPDATE cte 
		SET cte.[NINO] = cte.[CleanNINO]
		WHERE 
			cte.[NINO] <> cte.[CleanNINO] 
			AND cte.[CleanNINO] IS NOT NULL
		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END