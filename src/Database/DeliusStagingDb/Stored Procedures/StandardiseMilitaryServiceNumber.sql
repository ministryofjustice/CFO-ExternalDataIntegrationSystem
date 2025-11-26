CREATE PROCEDURE [DeliusStaging].[StandardiseMilitaryServiceNumber] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  

		;WITH cte AS(
			SELECT 
				MilitaryServiceNumber, 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseMilitaryServiceNumber](MilitaryServiceNumber) AS CleanMilitaryServiceNumber  
			FROM 
				[DeliusStaging].[AdditionalIdentifier] 
			WHERE 
				MilitaryServiceNumber IS NOT NULL
		)
		UPDATE cte 
			SET MilitaryServiceNumber = CleanMilitaryServiceNumber
		WHERE 
			MilitaryServiceNumber <> CleanMilitaryServiceNumber 
			AND CleanMilitaryServiceNumber IS NOT NULL

		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END