CREATE PROCEDURE [DeliusStaging].[StandardiseCRN] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  
		;WITH cte AS(
			SELECT 
				OffenderId, 
				CRN, 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseCRN](CRN) AS CleanCRN  
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE 
				CRN IS NOT NULL
		)
		UPDATE cte 
			SET cte.CRN = cte.CleanCRN
		WHERE 
			cte.CRN <> cte.CleanCRN 
			AND CleanCRN IS NOT NULL 
		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END
