CREATE PROCEDURE [OfflocStaging].[StandardisePrisonNumber] @rowCount INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	SET NOCOUNT ON;
	BEGIN TRY  
		--Prison Number 
		;with cte as(
		SELECT 
			  [OfflocStagingDb].[OfflocStaging].[Fn.StandardisePrisonNumber]([PrisonNumber])  as [CleanPrisonNumber]
			  ,[PrisonNumber]
		  FROM [OfflocStagingDb].[OfflocStaging].[Bookings]
		  )
		UPDATE cte
		SET [PrisonNumber] = [CleanPrisonNumber]
		where [PrisonNumber] <> [CleanPrisonNumber]
		and [CleanPrisonNumber] is not null

		--PreviousPrisonNumber
		;WITH cte AS(
			SELECT 
				[Details] as [PrisonNumber], 
				[OfflocStagingDb].[OfflocStaging].[Fn.StandardisePrisonNumber]([Details]) AS [CleanPrisonNumber]  
			FROM 
				[OfflocStagingDb].[OfflocStaging].[PreviousPrisonNumbers] 
			WHERE 
				[Details] IS NOT NULL
		)
		UPDATE cte 
			SET [PrisonNumber] = [CleanPrisonNumber]
		WHERE 
			[PrisonNumber] <> [CleanPrisonNumber] 
			AND [CleanPrisonNumber] IS NOT NULL
		
		SET @rowCount = @@ROWCOUNT;
	END TRY  
	BEGIN CATCH  
		SET @rowCount = -1;
	END CATCH;
		
	RETURN;
END