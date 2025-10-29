CREATE PROCEDURE [DeliusStaging].[StandardiseNomisNumber] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  
		;WITH cte AS(
			SELECT 
				[NOMISNumber], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseNomisNumber]([NOMISNumber]) AS [CleanNOMISNumber]  
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE 
				[NOMISNumber] IS NOT NULL
		)
		UPDATE cte 
			SET [NOMISNumber] = [CleanNOMISNumber]
		WHERE 
			[NOMISNumber] <> [CleanNOMISNumber] 
			AND [CleanNOMISNumber] IS NOT NULL
		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END