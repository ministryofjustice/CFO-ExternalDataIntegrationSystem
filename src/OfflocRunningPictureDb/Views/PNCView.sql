CREATE VIEW [OfflocRunningPicture].[PNCView]
	AS 

	SELECT
		DISTINCT	
		PD.NOMSnumber,
		SR.CleanedData AS [PNCNumber],
		'NOMIS' AS SourceName
	FROM         
		OfflocRunningPicture.PersonalDetails AS PD
		JOIN OfflocRunningPicture.StandardisationReference AS SR ON PD.NOMSnumber = SR.[Key] AND SR.[Type] = 'CleanPNCData'
GO

