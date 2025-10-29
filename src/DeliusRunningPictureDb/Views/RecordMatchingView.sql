CREATE VIEW [DeliusRunningPicture].[RecordMatchingView] 
AS 
SELECT 
	[CRN], 
	[OffenderId], 
	[FirstName_Raw], 
	[FirstName], 
	[SecondName_Raw], 
	[SecondName], 
	[Surname_Raw], 
	[Surname], 
	[CRO], 
	[NOMISNumber], 
	[PNCNumber], 
	[DateOfBirth], 
	[GenderDescription],
	'DELIUS' AS [SourceName]
FROM 
	[DeliusRunningPicture].[OffendersCleanView]
