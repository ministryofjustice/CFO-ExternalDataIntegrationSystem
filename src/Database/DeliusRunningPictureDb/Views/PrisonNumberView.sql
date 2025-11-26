CREATE VIEW [DeliusRunningPicture].[PrisonNumberView]
	AS 
	SELECT 
		DISTINCT
		O.CRN,
		SR.CleanedData AS PrisonNumber,
		'DELIUS' AS SourceName
	FROM         
		DeliusRunningPicture.Offenders AS O
		JOIN DeliusRunningPicture.StandardisationReference AS SR ON O.CRN = SR.[Key] AND SR.[Type] = 'CleanPrisonNumberData'
