CREATE PROCEDURE [OfflocStaging].[StandardisePhoneNumber] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  
		--Phone Number
		;WITH cte AS(
			SELECT 
				[Address7] as PhoneNumber, 
				[OfflocStagingDb].[OfflocStaging].[Fn.StandardisePhoneNumber]([Address7], 1) AS [CleanPhoneNumber]  
			FROM 
				[OfflocStagingDb].[OfflocStaging].[Addresses] 
			WHERE 
				[Address7] IS NOT NULL
		)
		UPDATE cte 
			SET [PhoneNumber] = [CleanPhoneNumber] 
		FROM cte
		WHERE 
			[PhoneNumber] <> [CleanPhoneNumber] 
			AND [CleanPhoneNumber] IS NOT NULL

		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END
