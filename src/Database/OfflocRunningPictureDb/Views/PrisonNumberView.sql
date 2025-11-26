CREATE VIEW [OfflocRunningPicture].[PrisonNumberView]
	AS 
	SELECT
		DISTINCT
		P.NOMSnumber,
		SR.CleanedData AS PrisonNumber,
		'NOMIS' as SourceName
	FROM 
		OfflocRunningPicture.PersonalDetails P
		JOIN OfflocRunningPicture.StandardisationReference AS SR ON P.NOMSnumber = SR.[Key] AND SR.[Type] = 'CleanPrisonNumberData'
