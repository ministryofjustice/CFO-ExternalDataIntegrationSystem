CREATE PROCEDURE [DeliusStaging].[StandardisePhoneNumber] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  
		--PHONE Number is stored AS ContactNo
		;WITH cte AS(
			SELECT 
				[ContactNo], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardisePhoneNumber]([ContactNo],1) AS [CleanContactNo]  
			FROM 
				[DeliusStaging].[OffenderManager] 
			WHERE 
				[ContactNo] IS NOT NULL
		)
		UPDATE cte 
			SET [ContactNo] = [CleanContactNo]
		WHERE 
			[ContactNo] <> [CleanContactNo] 
			AND [CleanContactNo] IS NOT NULL
	
		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END