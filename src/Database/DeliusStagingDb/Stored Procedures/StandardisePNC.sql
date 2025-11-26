CREATE PROCEDURE [DeliusStaging].[StandardisePNC] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  
		;WITH cte AS(
			SELECT 
				[PNCNumber], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardisePNC]([PNCNumber]) AS [CleanPNCNumber]  
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE [PNCNumber] IS NOT NULL
		)
		UPDATE cte 
			SET [PNCNumber] = [CleanPNCNumber]
		WHERE 
			[PNCNumber] <> [CleanPNCNumber] 
			AND [CleanPNCNumber] IS NOT NULL
	
		--Clean [PNC] in AdditionalIdentifier table
		;WITH cte AS(
			SELECT 
				[PNC], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardisePNC]([PNC]) AS [CleanPNC]  
			FROM 
				[DeliusStaging].[AdditionalIdentifier] 
			WHERE 
				[PNC] IS NOT NULL
		)
		UPDATE cte 
			SET [PNC] = [CleanPNC]
		WHERE 
			[PNC] <> [CleanPNC] 
			AND [CleanPNC] IS NOT NULL
	
		--Clean [OldPNC]] in AdditionalIdentifier table
		;WITH cte AS(
			SELECT 
				[OldPnc], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardisePNC]([OldPnc]) AS [CleanOldPnc]  
			FROM 
				[DeliusStaging].[AdditionalIdentifier] 
			WHERE 
				[OldPnc] IS NOT NULL
		)
		UPDATE cte 
			SET [OldPnc] = [CleanOldPnc]
		WHERE 
			[OldPnc] <> [CleanOldPnc] 
			AND [CleanOldPnc] IS NOT NULL
	
		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END
