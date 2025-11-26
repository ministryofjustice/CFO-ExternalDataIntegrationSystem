CREATE VIEW [DeliusRunningPicture].[CROView]
	AS 
	SELECT
		DISTINCT
		O.CRN,
		SR.CleanedData as CRO,
		'DELIUS' AS SourceName
	FROM         
		[DeliusRunningPicture].Offenders AS O
		JOIN [DeliusRunningPicture].StandardisationReference AS SR ON O.CRN = SR.[Key] AND SR.[Type]='CleanCROData'