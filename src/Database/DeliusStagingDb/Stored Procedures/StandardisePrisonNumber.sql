CREATE PROCEDURE [DeliusStaging].[StandardisePrisonNumber] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  
		;WITH cte AS(
			SELECT 
				[PrisonNumber], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardisePNC]([PrisonNumber]) AS [CleanPrisonNumber]  
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE 
				[PrisonNumber] IS NOT NULL
		)
		UPDATE cte 
			SET [PrisonNumber] = [CleanPrisonNumber]
		WHERE 
			[PrisonNumber] <> [CleanPrisonNumber] 
			AND [CleanPrisonNumber] IS NOT NULL
		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END