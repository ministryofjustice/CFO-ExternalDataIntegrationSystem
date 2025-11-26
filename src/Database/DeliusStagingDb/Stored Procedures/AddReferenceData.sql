CREATE PROCEDURE [DeliusStaging].[AddReferenceData] @retVal INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
    	/***************************************************Reference Data****************************************************/

		/*****************************************Delius Reference Data START*************************************************/
		--New Gender not seen before
		;WITH cteGender AS(
		SELECT 
			DISTINCT 
			GenderCode, 
			GenderDescription, 
			[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseGender](GenderCode) AS CleanGender
		FROM 
			[DeliusStagingDb].[DeliusStaging].[Offenders] o
			LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
			ON sr.[RawData] = o.GenderCode AND sr.[Type] = 'NewData'
		WHERE 
			sr.RawData IS NULL
			AND GenderCode IS NOT NULL
		)
		INSERT INTO  [DeliusStagingDb].[DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cteGender.GenderCode, 
			cteGender.CleanGender, 
			'[Offenders].GenderCode' AS [Source], 
			'NewData' AS [Type] 
		FROM 
			cteGender 
		WHERE CleanGender = 'other';

		--New Ethinicity, not seen before
		;WITH cteEthnicity AS (
		SELECT 
			DISTINCT [EthnicityDescription], 
			[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseEthnicity]([EthnicityDescription]) AS [CleanEthnicityDescription] 
		FROM 
			[DeliusStagingDb].[DeliusStaging].[Offenders] o
			LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
			ON sr.[RawData] = o.[EthnicityDescription] AND sr.[Type] = 'NewData'
		WHERE 
			sr.RawData IS NULL
			AND o.[EthnicityDescription] is not null
		)
		INSERT INTO  [DeliusStagingDb].[DeliusStaging].StandardisationReference ([RawData], [Source], [Type])
		SELECT 
			cteEthnicity.[EthnicityDescription],  
			'[Offenders].EthnicityDescription' AS [Source], 
			'NewData' AS [Type]
		FROM cteEthnicity 
		WHERE CleanEthnicityDescription = 'refused / not stated / new ethnicity';

		--Nationality, not seen before
		;WITH cteNationality AS(
			SELECT
				DISTINCT
				[NationalityDescription], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseNationality]([NationalityDescription]) AS [CleanNationalityDescription] 
			FROM
				[DeliusStagingDb].[DeliusStaging].[Offenders] o
				LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = o.[NationalityDescription] AND sr.[Type] = 'NewData'
			WHERE 
				sr.RawData IS NULL
				AND o.[NationalityDescription] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [Source], [Type])
		SELECT 
			cteNationality.[NationalityDescription],  
			'[Offenders].NationalityDescription' AS [Source], 
			'NewData' AS [Type]
		FROM cteNationality
		WHERE 
			cteNationality.[CleanNationalityDescription] IS NULL

		--Nationality, not seen before
		;WITH cteNationality AS(
			SELECT
				DISTINCT
				[SecondNationalityDescription], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseNationality]([SecondNationalityDescription]) AS [CleanSecondNationalityDescription] 
			FROM
				[DeliusStagingDb].[DeliusStaging].[Offenders] o
				LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = o.[SecondNationalityDescription] AND sr.[Type] = 'NewData'
			WHERE 
				sr.RawData IS NULL
				AND o.[SecondNationalityDescription] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [Source], [Type])
		SELECT 
			cteNationality.[SecondNationalityDescription],  
			'[Offenders].SecondNationalityDescription' AS [Source], 
			'NewData' AS [Type]
		FROM cteNationality
		WHERE 
			cteNationality.[CleanSecondNationalityDescription] IS NULL

		/*Add Clean Names-Raw Names to reference Table*/
		--Clean [FirstName] in AliasDetails table
		;WITH cte AS(
			SELECT 
				DISTINCT [FirstName], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseName]([FirstName]) AS [CleanFirstName] 
			FROM 
				[DeliusStagingDb].[DeliusStaging].[AliasDetails] ad
				LEFT JOIN [DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = ad.[FirstName] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND ad.[FirstName] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.FirstName,
			cte.CleanFirstName,
			'[AliasDetails].FirstName',
			'CleanNameData'
		FROM cte 
		WHERE 
			[FirstName] <> [CleanFirstName] 
			AND [CleanFirstName] IS NOT NULL
	
		--Clean [SecondName] in AliasDetails table
		;WITH cte AS(
			SELECT
				DISTINCT
				[SecondName], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseName]([SecondName]) AS [CleanSecondName]  
			FROM 
				[DeliusStaging].[AliasDetails] ad
				LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = ad.[SecondName] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND [SecondName] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.SecondName ,
			cte.CleanSecondName,
			'[AliasDetails].SecondName',
			'CleanNameData'
		FROM cte
		WHERE 
			[SecondName] <> [CleanSecondName] 
			AND [CleanSecondName] IS NOT NULL
	
		--Clean [ThirdName] in AliasDetails table
		;WITH cte AS(
			SELECT 
				DISTINCT
				[ThirdName], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseName]([ThirdName]) AS [CleanThirdName]  
			FROM 
				[DeliusStaging].[AliasDetails] ad
				LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = ad.[ThirdName] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND [ThirdName] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.ThirdName,
			cte.CleanThirdName,
			'[AliasDetails].ThirdName',
			'CleanNameData'
		FROM cte
		WHERE 
			[ThirdName] <> [CleanThirdName]
			AND [CleanThirdName] IS NOT NULL
	
		--Clean [Surname] in AliasDetails table
		;WITH cte AS(
			SELECT 
				DISTINCT
				[Surname], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseName]([Surname]) AS [CleanSurName]  
			FROM 
				[DeliusStaging].[AliasDetails] ad
				LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = ad.[Surname] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND [Surname] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.Surname, 
			cte.CleanSurName,
			'[AliasDetails].SurName',
			'CleanNameData'
		FROM cte
		WHERE 
			[Surname] <> [CleanSurName]
			AND [CleanSurName] IS NOT NULL
	
		--Clean [OmForename] in OffenderManager table
		;WITH cte AS(
			SELECT 
				DISTINCT
				[OmForename], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseName]([OmForename]) AS [CleanOmForename]  
			FROM 
				[DeliusStaging].[OffenderManager] om
				LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = om.[OmForename] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND [OmForename] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.OmForename, 
			cte.CleanOmForename,
			'[AliasDetails].OmForename',
			'CleanNameData'
		FROM cte
		WHERE 
			[OmForename] <> [CleanOmForename]
			AND [CleanOmForename] IS NOT NULL
	
		--Clean [OmSurname] in OffenderManager table
		;WITH cte AS(
			SELECT 
				DISTINCT
				[OmSurname], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseName]([OmSurname]) AS [CleanOmSurname]  
			FROM 
				[DeliusStaging].[OffenderManager] om
				LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = om.[OmSurname] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND [OmSurname] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.OmSurname, 
			cte.CleanOmSurname,
			'[OffenderManager].OmSurname',
			'CleanNameData'
		FROM cte
		WHERE 
			[OmSurname] <> [CleanOmSurname] 
			AND [CleanOmSurname] IS NOT NULL
	
		--Clean [FirstName] in Offenders table
		;WITH cte AS(
			SELECT 
				DISTINCT
				[FirstName], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseName]([FirstName]) AS [CleanFirstName]  
			FROM 
				[DeliusStaging].[Offenders] o
				LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = o.[FirstName] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND o.[FirstName] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.FirstName, 
			cte.CleanFirstName,
			'[Offenders].FirstName',
			'CleanNameData'
		FROM cte
		WHERE 
			[FirstName] <> [CleanFirstName] 
			AND [CleanFirstName] IS NOT NULL
	
		--Clean [SecondName] in Offenders table
		;WITH cte AS(
			SELECT 
				DISTINCT
				[SecondName], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseName]([SecondName]) AS [CleanSecondName]  
			FROM 
				[DeliusStaging].[Offenders] o
				LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = o.[SecondName] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND o.[SecondName] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.SecondName, 
			cte.CleanSecondName,
			'[Offenders].SecondName',
			'CleanNameData'
		FROM cte
		WHERE 
			[SecondName] <> [CleanSecondName] 
			AND [CleanSecondName] IS NOT NULL
	
		--Clean [ThirdName] in Offenders table
		;WITH cte AS(
			SELECT 
				DISTINCT
				[ThirdName], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseName]([ThirdName]) AS [CleanThirdName]  
			FROM 
				[DeliusStaging].[Offenders] o
				LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = o.[ThirdName] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND o.[ThirdName] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Source], [Type])
		SELECT 	
			cte.ThirdName, 
			cte.CleanThirdName,
			'[Offenders].ThirdName',
			'CleanNameData'
		FROM cte
		WHERE 
			[ThirdName] <> [CleanThirdName]
			AND [CleanThirdName] IS NOT NULL
	
		--Clean [Surname] in Offenders table
		;WITH cte AS(
			SELECT 
				DISTINCT
				[Surname], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseName]([Surname]) AS [CleanSurname]  
			FROM 
				[DeliusStaging].[Offenders] o
				LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = o.[Surname] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND o.[Surname] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.Surname, 
			cte.CleanSurname,
			'[Offenders].Surname',
			'CleanNameData'
		FROM cte
		WHERE 
			[Surname] <> [CleanSurname] 
			AND [CleanSurname] IS NOT NULL
	
		--Clean [PreviousSurname] in Offenders table
		;WITH cte AS(
			SELECT 
				DISTINCT
				[PreviousSurname], 
				[DeliusStagingDb].[DeliusStaging].[Fn.StandardiseName]([PreviousSurname]) AS [CleanPreviousSurname]  
			FROM 
				[DeliusStaging].[Offenders] o
				LEFT JOIN [DeliusStagingDb].[DeliusStaging].[StandardisationReference] sr 
				ON sr.[RawData] = o.[PreviousSurname] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND o.[PreviousSurname] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.PreviousSurname, 
			cte.CleanPreviousSurname,
			'[Offenders].PreviousSurname',
			'CleanNameData'
		FROM cte
		WHERE 
			[PreviousSurname] <> [CleanPreviousSurname]
			AND [CleanPreviousSurname] IS NOT NULL
	
		--Clean [CRO] in Offenders table
		;WITH cte AS(
			SELECT 
				DISTINCT
				[CRO],
				[DeliusStaging].[Fn.StandardiseCRO]([CRO]) AS CleanCRO,
				[CRN]
			FROM 
				[DeliusStaging].[Offenders]
			WHERE 
				[CRO] IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Key], [KeyColumnName], [Source], [Type])
		SELECT 
			cte.[CRO] as [RawData], 
			cte.CleanCRO as [CleanedData],
			cte.[CRN] as [Key],
			'CRN' as [KeyColumnName],
			'[Offenders].CRO' as [Source],
			'CleanCROData' as [Type]
		FROM cte
		WHERE 
			CleanCRO IS NOT NULL
		
		--Clean [PNC] in Offenders table
		;WITH cte AS(
			SELECT 
				DISTINCT 
				[PNCNumber],
				[DeliusStaging].[Fn.StandardisePNC](PNCNumber) AS CleanPNC,
				[CRN]  
			FROM 
				[DeliusStaging].[Offenders]
			WHERE 
				PNCNumber IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Key], [KeyColumnName], [Source], [Type])
		SELECT 
			cte.[PNCNumber] as [RawData], 
			cte.CleanPNC as [CleanedData],
			cte.[CRN] as [Key],
			'CRN' as [KeyColumnName],
			'[Offenders].PNCNumber',
			'CleanPNCData'
		FROM cte
		WHERE 
			cte.CleanPNC IS NOT NULL
		
		--Clean [PrisonNumber] in Offenders table
		;WITH cte AS(
			SELECT 
				DISTINCT 
				[PrisonNumber],
				[DeliusStaging].[Fn.StandardisePrisonNumber](PrisonNumber) AS [CleanPrisonNumber],
				[CRN]
			FROM 
				[DeliusStaging].[Offenders]
			WHERE 
				PrisonNumber IS NOT NULL
		)
		INSERT INTO  [DeliusStaging].StandardisationReference ([RawData], [CleanedData], [Key], [KeyColumnName], [Source], [Type])
		SELECT 
			cte.[PrisonNumber] as [RawData], 
			cte.[CleanPrisonNumber] as [CleanedData],
			cte.[CRN] as [Key],
			'CRN' as [KeyColumnName],
			'[Offenders].PrisonNumber' as [Source],
			'CleanPrisonNumberData' as [Type]
		FROM cte
		WHERE 
			cte.[CleanPrisonNumber] IS NOT NULL			

		/*****************************************Delius Reference Data END***************************************************/

		--Remove duplicates
		;WITH cte as
		(
			SELECT 
				Row_number() OVER(PARTITION BY  [RawData], [CleanedData], [Key], [KeyColumnName] ORDER BY [Key], [KeyColumnName]) RowNum, 
				[RawData], 
				[CleanedData]  
			FROM 
				[DeliusStaging].[StandardisationReference] 
		)DELETE FROM cte WHERE RowNum > 1

	END TRY
	BEGIN CATCH
		--THROW;
		SET @retVal = -1;
	END CATCH
RETURN;
END