CREATE PROCEDURE [OfflocStaging].[StandardisePostCode] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  
		--Postcode
		;WITH cte AS(
			SELECT 
				[Address6] as Postcode, 
				[OfflocStagingDb].[OfflocStaging].[Fn.StandardisePostCode]([Address6]) AS [CleanPostcode]  
			FROM 
				[OfflocStagingDb].[OfflocStaging].[Addresses] 
			WHERE 
				[Address6] IS NOT NULL
		)
		UPDATE cte 
			SET [Postcode] = [CleanPostcode] 
		FROM cte
		WHERE 
			[Postcode] <> [CleanPostcode] 
			AND [CleanPostcode] IS NOT NULL
	
		--[DeliusStaging].[StandardisePrisonNumber] -END
		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END

