CREATE VIEW [OfflocRunningPicture].[CROView]
	AS 
	SELECT
		DISTINCT
		P.NOMSnumber,
		SR.CleanedData as CRO,
		'NOMIS' AS SourceName
	FROM         
		OfflocRunningPicture.PersonalDetails AS P
		JOIN OfflocRunningPicture.StandardisationReference AS SR ON P.NOMSnumber = SR.[Key] AND SR.[Type]='CleanCROData'