CREATE PROCEDURE [OfflocStaging].[StandardiseCRO] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  
		--CRO
		;WITH cte AS(
			SELECT 
				NOMSnumber, 
				CROno AS CRO, 
				[OfflocStagingDb].[OfflocStaging].[Fn.StandardiseCRO](CROno) AS CleanCRO  
			FROM 
				[OfflocStagingDb].[OfflocStaging].[Identifiers] 
			WHERE 
				CROno IS NOT NULL
		)
		UPDATE cte 
			SET CRO = CleanCRO
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
