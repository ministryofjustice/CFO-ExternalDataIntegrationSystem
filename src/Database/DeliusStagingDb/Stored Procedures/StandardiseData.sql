Create PROCEDURE [DeliusStaging].[StandardiseData] @retMessage varchar(200) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result SETs FROM
	-- interfering WITH SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION;	
	BEGIN TRY
		Declare @retVal int = 0;

		EXEC [DeliusStaging].[InitializeStandardisationLookups];

		/***************************************************Delius Standardisation START**************************************/		

		EXEC [DeliusStaging].[StandardiseCRN] @retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Delius Staging StandardiseCRN';
		END
		
		EXEC [DeliusStaging].[StandardiseCRO]	@retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Delius Staging StandardiseCRO';
		END

		EXEC [DeliusStaging].[StandardiseEthnicity] @retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Delius Staging StandardiseEthnicity';
		END

		EXEC [DeliusStaging].[StandardiseGender]	@retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Delius Staging StandardiseGender';
		END

		EXEC [DeliusStaging].[StandardiseMilitaryServiceNumber]	@retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Delius Staging StandardiseMilitaryServiceNumber';
		END

		EXEC [DeliusStaging].[StandardiseNationality]	@retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Delius Staging StandardiseNationality';
		END

		EXEC [DeliusStaging].[StandardiseNINO] @retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Delius Staging StandardiseNINO';
		END	

		EXEC [DeliusStaging].[StandardiseNomisNumber]	@retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Delius Staging StandardiseNomisNumber';
		END

		EXEC [DeliusStaging].[StandardisePhoneNumber]	@retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Delius Staging StandardisePhoneNumber';
		END

		EXEC [DeliusStaging].[StandardisePNC]	@retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Delius Staging StandardisePNC';
		END

		/*
		--[DeliusStaging].[StandardiseName] -START
	
		--Clean [FirstName] in AliasDetails table
		;WITH cte AS(
			SELECT 
				[FirstName], 
				[DeliusStaging].[StandardiseName]([FirstName]) AS [CleanFirstName]  
			FROM 
				[DeliusStaging].[AliasDetails] 
			WHERE 
				[FirstName] IS NOT NULL
		)
		UPDATE cte 
			SET [FirstName] = [CleanFirstName]
		WHERE 
			[FirstName] <> [CleanFirstName] 
			AND [CleanFirstName] IS NOT NULL
	
		COMMIT TRANSACTION;
	
		--Clean [SecondName] in AliasDetails table
		;WITH cte AS(
			SELECT 
				[SecondName], 
				[DeliusStaging].[StandardiseName]([SecondName]) AS [CleanSecondName]  
			FROM 
				[DeliusStaging].[AliasDetails] 
			WHERE 
				[SecondName] IS NOT NULL
		)
		UPDATE cte 
			SET [SecondName] = [CleanSecondName]
		WHERE 
			[SecondName] <> [CleanSecondName] 
			AND [CleanSecondName] IS NOT NULL
	
		COMMIT TRANSACTION;
	
		--Clean [ThirdName] in AliasDetails table
		;WITH cte AS(
			SELECT 
				[ThirdName], 
				[DeliusStaging].[StandardiseName]([ThirdName]) AS [CleanThirdName]  
			FROM 
				[DeliusStaging].[AliasDetails] 
			WHERE 
				[ThirdName] IS NOT NULL
		)
		UPDATE cte 
			SET [ThirdName] = [CleanThirdName]
		WHERE 
			[ThirdName] <> [CleanThirdName] 
			AND [CleanThirdName] IS NOT NULL
	
		COMMIT TRANSACTION;
	
		--Clean [Surname] in AliasDetails table
		;WITH cte AS(
			SELECT 
				[Surname], 
				[DeliusStaging].[StandardiseName]([Surname]) AS [CleanSurName]  
			FROM 
				[DeliusStaging].[AliasDetails] 
			WHERE 
				[Surname] IS NOT NULL
		)
		UPDATE cte 
			SET [Surname] = [CleanSurName]
		WHERE 
			[Surname] <> [CleanSurName] 
			AND [CleanSurName] IS NOT NULL
	
		COMMIT TRANSACTION;
	
		--Clean [OmForename] in OffenderManager table
		;WITH cte AS(
			SELECT 
				[OmForename], 
				[DeliusStaging].[StandardiseName]([OmForename]) AS [CleanOmForename]  
			FROM 
				[DeliusStaging].[OffenderManager] 
			WHERE 
				[OmForename] IS NOT NULL
		)
		UPDATE cte 
			SET [OmForename] = [CleanOmForename]
		WHERE 
			[OmForename] <> [CleanOmForename]
			AND [CleanOmForename] IS NOT NULL
	
		COMMIT TRANSACTION;
	
		--Clean [OmSurname] in OffenderManager table
		;WITH cte AS(
			SELECT 
				[OmSurname], 
				[DeliusStaging].[StandardiseName]([OmSurname]) AS [CleanOmSurname]  
			FROM 
				[DeliusStaging].[OffenderManager] 
			WHERE 
				[OmSurname] IS NOT NULL
		)
		UPDATE cte 
			SET [OmSurname] = [CleanOmSurname]
		WHERE 
			[OmSurname] <> [CleanOmSurname] 
			AND [CleanOmSurname] IS NOT NULL
	
		COMMIT TRANSACTION;
	
		--Clean [FirstName] in Offenders table
		;WITH cte AS(
			SELECT 
				[FirstName], 
				[DeliusStaging].[StandardiseName]([FirstName]) AS [CleanFirstName]  
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE 
			[FirstName] IS NOT NULL
		)
		UPDATE cte 
			SET [FirstName] = [CleanFirstName]
		WHERE 
			[FirstName] <> [CleanFirstName] 
			AND [CleanFirstName] IS NOT NULL
	
		COMMIT TRANSACTION;
	
		--Clean [SecondName] in Offenders table
		;WITH cte AS(
			SELECT 
				[SecondName], 
				[DeliusStaging].[StandardiseName]([SecondName]) AS [CleanSecondName]  
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE 
				[SecondName] IS NOT NULL
		)
		UPDATE cte 
			SET [SecondName] = [CleanSecondName]
		WHERE 
			[SecondName] <> [CleanSecondName] 
			AND [CleanSecondName] IS NOT NULL
	
		COMMIT TRANSACTION;
	
		--Clean [ThirdName] in Offenders table
		;WITH cte AS(
			SELECT 
				[ThirdName], 
				[DeliusStaging].[StandardiseName]([ThirdName]) AS [CleanThirdName]  
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE 
				[ThirdName] IS NOT NULL
		)
		UPDATE cte 
			SET [ThirdName] = [CleanThirdName]
		WHERE 
			[ThirdName] <> [CleanThirdName] 
			AND [CleanThirdName] IS NOT NULL
	
		COMMIT TRANSACTION;
	
		--Clean [Surname] in Offenders table
		;WITH cte AS(
			SELECT 
				[Surname], 
				[DeliusStaging].[StandardiseName]([Surname]) AS [CleanSurname]  
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE 
				[Surname] IS NOT NULL
		)
		UPDATE cte 
			SET [Surname] = [CleanSurname]
		WHERE 
			[Surname] <> [CleanSurname] 
			AND [CleanSurname] IS NOT NULL
	
		COMMIT TRANSACTION;
	
		--Clean [PreviousSurname] in Offenders table
		;WITH cte AS(
			SELECT 
				[PreviousSurname], 
				[DeliusStaging].[StandardiseName]([PreviousSurname]) AS [CleanPreviousSurname]  
			FROM 
				[DeliusStaging].[Offenders] 
			WHERE 
				[PreviousSurname] IS NOT NULL
		)
		UPDATE cte 
			SET [PreviousSurname] = [CleanPreviousSurname]
		WHERE 
			[PreviousSurname] <> [CleanPreviousSurname] 
			AND [CleanPreviousSurname] IS NOT NULL
	
		COMMIT TRANSACTION;
	
		--[DeliusStaging].[StandardiseName] -END
		

		*/
		--Commented above code which standardises names, now keeping the raw data in the staging and running picture dbs
		--Adding the cleaned names in a reference table which is in one of the sections below 

		/***************************************************Delius Standardisation END****************************************/


		/***************************************************Delius Add Reference Data START****************************************/
		EXEC [DeliusStaging].[AddReferenceData] @retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Delius Staging AddReferenceData';
		END
		/***************************************************Delius Add Reference Data END******************************************/

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH
ENd