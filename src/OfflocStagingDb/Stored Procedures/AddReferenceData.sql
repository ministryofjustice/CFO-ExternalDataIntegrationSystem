CREATE PROCEDURE [OfflocStaging].[AddReferenceData]  @retVal INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	set @retVal=0;

		/*****************************************Offloc Reference Data START*************************************************/
	BEGIN TRY
		--New Gender not seen before
		;WITH cteGender AS(
		SELECT 
			DISTINCT 
			Gender, 
			[OfflocStaging].[Fn.StandardiseGender](Gender) AS CleanGender
		FROM 
			[OfflocStagingDb].[OfflocStaging].[PersonalDetails] p
			LEFT JOIN [OfflocStagingDb].[OfflocStaging].[StandardisationReference] sr 
			ON sr.[RawData] = p.Gender AND sr.[Type] = 'NewData'
		WHERE 
			sr.RawData IS NULL
			AND Gender IS NOT NULL
		)
		INSERT INTO  [OfflocStagingDb].[OfflocStaging].[StandardisationReference] ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cteGender.Gender, 
			cteGender.CleanGender, 
			'[PersonalDetails].Gender' AS [Source], 
			'NewData' AS [Type] 
		FROM 
			cteGender 
		WHERE CleanGender = 'other';

		--New Ethinicity, not seen before
		;WITH cteEthnicity AS (
		SELECT 
			DISTINCT [EthnicGroup], 
			[OfflocStagingDb].[OfflocStaging].[Fn.StandardiseEthnicity]([EthnicGroup]) AS [CleanEthnicGroup] 
		FROM 
			[OfflocStagingDb].[OfflocStaging].[PersonalDetails] p
			LEFT JOIN [OfflocStagingDb].[OfflocStaging].[StandardisationReference] sr 
			ON sr.[RawData] = p.[EthnicGroup] AND sr.[Type] = 'NewData'
		WHERE 
			sr.RawData IS NULL
			AND p.[EthnicGroup] is not null
		)
		INSERT INTO  [OfflocStaging].[StandardisationReference] ([RawData], [Source], [Type])
		SELECT 
			cteEthnicity.[EthnicGroup],  
			'[PersonalDetails].EthnicGroup' AS [Source], 
			'NewData' AS [Type]
		FROM cteEthnicity 
		WHERE CleanEthnicGroup = 'refused / not stated / new ethnicity';


		--Nationality, not seen before
		;WITH cteNationality AS(
			SELECT
				DISTINCT
				[Nationality], 
				[OfflocStaging].[Fn.StandardiseNationality]([Nationality]) AS [CleanNationality] 
			FROM
				[OfflocStagingDb].[OfflocStaging].[PersonalDetails] p
				LEFT JOIN [OfflocStagingDb].[OfflocStaging].[StandardisationReference] sr 
				ON sr.[RawData] = p.[Nationality] AND sr.[Type] = 'NewData'
			WHERE 
				sr.RawData IS NULL
				AND p.[Nationality] IS NOT NULL
		)
		INSERT INTO  [OfflocStaging].[StandardisationReference] ([RawData], [Source], [Type])
		SELECT 
			cteNationality.[Nationality],  
			'[PersonalDetails].Nationality' AS [Source], 
			'NewData' AS [Type]
		FROM cteNationality
		WHERE 
			cteNationality.[CleanNationality] IS NULL

		/*Add Clean Names-Raw Names to reference Table*/
		--Clean [ForeName1] in PersonalDetails table
		;WITH cte AS(
			SELECT 
				DISTINCT [Forename1], 
				[OfflocStaging].[Fn.StandardiseName]([Forename1]) AS [CleanForeName1] 
			FROM 
				[OfflocStagingDb].[OfflocStaging].[PersonalDetails] p
				LEFT JOIN [OfflocStaging].[StandardisationReference] sr 
				ON sr.[RawData] = p.[Forename1] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND p.[Forename1] IS NOT NULL
		)
		INSERT INTO  [OfflocStaging].[StandardisationReference] ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.Forename1,
			cte.CleanForeName1,
			'[PersonalDetails].Forename1',
			'CleanNameData'
		FROM cte 
		WHERE 
			[Forename1] <> [CleanForeName1] 
			AND [CleanForeName1] IS NOT NULL
	
		--Clean [ForeName2] in PersonalDetails table
		;WITH cte AS(
			SELECT 
				DISTINCT [Forename2], 
				[OfflocStaging].[Fn.StandardiseName]([Forename2]) AS [CleanForeName2] 
			FROM 
				[OfflocStagingDb].[OfflocStaging].[PersonalDetails] p
				LEFT JOIN [OfflocStaging].[StandardisationReference] sr 
				ON sr.[RawData] = p.[Forename2] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND p.[Forename2] IS NOT NULL
		)
		INSERT INTO  [OfflocStaging].[StandardisationReference] ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.Forename2,
			cte.CleanForeName2,
			'[PersonalDetails].Forename2',
			'CleanNameData'
		FROM cte 
		WHERE 
			[Forename2] <> [CleanForeName2] 
			AND [CleanForeName2] IS NOT NULL
	
		--Clean [Surname] in PersonalDetails table
		;WITH cte AS(
			SELECT 
				DISTINCT [Surname], 
				[OfflocStaging].[Fn.StandardiseName]([Surname]) AS [CleanSurname]
			FROM 
				[OfflocStagingDb].[OfflocStaging].[PersonalDetails] p
				LEFT JOIN [OfflocStaging].[StandardisationReference] sr 
				ON sr.[RawData] = p.[Surname] AND sr.[Type] = 'CleanNameData'
			WHERE 
				sr.RawData IS NULL
				AND p.[Surname] IS NOT NULL
		)
		INSERT INTO  [OfflocStaging].[StandardisationReference] ([RawData], [CleanedData], [Source], [Type])
		SELECT 
			cte.[Surname],
			cte.[CleanSurname],
			'[PersonalDetails].Surname',
			'CleanNameData'
		FROM cte 
		WHERE 
			[Surname] <> [CleanSurname]
			AND [CleanSurname] IS NOT NULL

		--Clean [CRO] in Identifiers table
		; with cte as (
			SELECT 
				DISTINCT [OfflocStaging].[Fn.StandardiseCRO](CROno) AS [CleanCRO],
				NOMSnumber
			FROM 
				[OfflocStaging].[Identifiers]
			WHERE 
				[CROno] IS NOT NULL
		)
		INSERT INTO  [OfflocStaging].[StandardisationReference] ([RawData], [CleanedData], [Key], [KeyColumnName], [Source], [Type])
		SELECT 
			cte.[CleanCRO] AS [RawData],
			cte.[CleanCRO] AS [CleanedData],
			cte.[NOMSnumber] AS [Key],
			'NOMSnumber' AS [KeyColumnName],
			'[Identifiers].CROno' AS [Source],
			'CleanCROData' AS [Type]
		FROM cte 
		WHERE 
		cte.[CleanCRO] IS NOT NULL

		--Clean [PNC] in PNC table
		;WITH cte AS(
			SELECT 
				DISTINCT [OfflocStaging].[Fn.StandardisePNC](Details) AS [CleanPNC],
				[NOMSnumber]
			FROM 
				[OfflocStagingDb].[OfflocStaging].[PNC]
			WHERE 
				[Details] IS NOT NULL
		)
		INSERT INTO  [OfflocStaging].[StandardisationReference] ([RawData], [CleanedData], [Key], [KeyColumnName], [Source], [Type])
		SELECT 
			cte.[CleanPNC] AS [RawData],
			cte.[CleanPNC] AS [CleanedData],
			cte.[NOMSnumber] AS [Key],
			'NOMSnumber' AS [KeyColumnName],
			'[PNC].Details' AS [Source],
			'CleanPNCData' AS [Type]
		FROM cte 
		WHERE 
			cte.[CleanPNC] IS NOT NULL

		--Clean [PrisonNumber] in Bookings table
		;WITH cte AS(
			SELECT 
				DISTINCT [OfflocStaging].[Fn.StandardisePrisonNumber](PrisonNumber) AS [CleanPrisonNumber],
				[NOMSnumber]
			FROM 
				[OfflocStagingDb].[OfflocStaging].[Bookings]
			WHERE 
				[PrisonNumber] IS NOT NULL
		)
		INSERT INTO  [OfflocStaging].[StandardisationReference] ([RawData], [CleanedData], [Key], [KeyColumnName], [Source], [Type])
		SELECT 
			cte.[CleanPrisonNumber] AS [RawData],
			cte.[CleanPrisonNumber] AS [CleanedData],
			cte.[NOMSnumber] AS [Key],
			'NOMSnumber' AS [KeyColumnName],
			'[Bookings].PrisonNumber' AS [Source],
			'CleanPrisonNumberData' AS [Type]
		FROM cte 
		WHERE 
			CleanPrisonNumber IS NOT NULL

		/*****************************************Offloc Reference Data END***************************************************/

		--remove duplicates
		;WITH cte as
		(
			SELECT 
				Row_number() OVER(PARTITION BY  RawData, CleanedData, [Key], [KeyColumnName] ORDER BY [Key], [KeyColumnName]) RowNum, 
				RawData, 
				CleanedData  
			FROM 
				[OfflocStaging].[StandardisationReference] 
		)DELETE FROM cte WHERE RowNum > 1

	END TRY
	BEGIN CATCH
		--THROW;
		SET @retVal = -1;
	END CATCH
RETURN;
END
