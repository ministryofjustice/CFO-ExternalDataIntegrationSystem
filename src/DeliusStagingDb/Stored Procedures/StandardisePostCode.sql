CREATE PROCEDURE [DeliusStaging].[StandardisePostCode] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  

		--Clean OffenderAddress table
		;WITH cte AS(
			SELECT 
				[Postcode], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardisePostCode]([Postcode]) AS [CleanPostcode]  
			FROM 
				[DeliusStaging].[OffenderAddress] 
			WHERE 
				[Postcode] IS NOT NULL
		)
		UPDATE cte 
			SET [Postcode] = [CleanPostcode]
		WHERE 
			[Postcode] <> [CleanPostcode] 
			AND [CleanPostcode] IS NOT NULL

		--Clean OffenderManagerBuildings table
		;WITH cte AS(
			SELECT 
				[Postcode], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardisePostCode]([Postcode]) AS [CleanPostcode]  
			FROM 
				[DeliusStaging].[OffenderManagerBuildings] 
			WHERE 
				[Postcode] IS NOT NULL
		)
		UPDATE cte 
			SET [Postcode] = [CleanPostcode]
		WHERE 
			[Postcode] <> [CleanPostcode] 
			AND [CleanPostcode] IS NOT NULL
		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END

