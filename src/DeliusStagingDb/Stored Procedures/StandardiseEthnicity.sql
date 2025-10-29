CREATE PROCEDURE [DeliusStaging].[StandardiseEthnicity] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  

		;WITH cte AS(
			SELECT 
				[EthnicityDescription],	
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseEthnicity]([EthnicityDescription]) AS [CleanEthnicityDescription] 
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE
				[EthnicityDescription] IS NOT NULL
		)
		UPDATE cte 
			SET cte.[EthnicityDescription] = cte.[CleanEthnicityDescription]
		WHERE 
			cte.[EthnicityDescription] <> cte.[CleanEthnicityDescription]
			AND cte.[CleanEthnicityDescription] IS NOT NULL
		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END