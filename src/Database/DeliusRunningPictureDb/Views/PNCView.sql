CREATE VIEW [DeliusRunningPicture].[PNCView]
	AS 

	SELECT 
		DISTINCT
		O.CRN,
		SR.CleanedData AS [PNCNumber],
		'DELIUS' AS SourceName
	FROM         
		DeliusRunningPicture.Offenders AS O
		JOIN DeliusRunningPicture.StandardisationReference AS SR ON O.CRN = SR.[Key] AND SR.[Type] = 'CleanPNCData'
GO

