CREATE PROCEDURE [DeliusStaging].[StandardiseCRO] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  
		--[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseCRO]
		--clean CRO in Offenders table
		;WITH cte AS(
			SELECT 
				OffenderId, 
				CRO, 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseCRO](CRO) AS CleanCRO  
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE 
				CRO IS NOT NULL
		)
		UPDATE cte 
			SET cte.CRO = cte.CleanCRO
		WHERE 
			cte.CRO <> cte.CleanCRO 
			AND CleanCRO IS NOT NULL
		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END
