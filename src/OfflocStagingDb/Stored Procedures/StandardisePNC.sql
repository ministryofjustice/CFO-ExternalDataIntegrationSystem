CREATE PROCEDURE [OfflocStaging].[StandardisePNC] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  
		--PNC
		;WITH cte AS(
			SELECT 
				[Details] as PNC, 
				[OfflocStagingDb].[OfflocStaging].[Fn.StandardisePNC]([Details]) AS [CleanPNC]  
			FROM 
				[OfflocStagingDb].[OfflocStaging].[PNC]
			WHERE 
				[Details] IS NOT NULL
		)
		UPDATE cte 
			SET [PNC] = [CleanPNC]
		WHERE 
			[PNC] <> [CleanPNC] 
			AND [CleanPNC] IS NOT NULL
		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END

