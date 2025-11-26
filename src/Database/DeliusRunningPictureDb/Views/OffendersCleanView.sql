CREATE VIEW [DeliusRunningPicture].[OffendersCleanView]
AS
SELECT DISTINCT
       [CRN],
       [OffenderId],
       [FirstName] AS FirstName_Raw,
       LOWER(COALESCE(frstn.[CleanedData], o.[FirstName])) AS FirstName,
       [SecondName] AS SecondName_Raw,
       LOWER(COALESCE(secn.[CleanedData], o.[SecondName])) AS SecondName,
       [Surname] AS SurName_Raw,
       LOWER(COALESCE(surn.[CleanedData], o.[Surname])) AS SurName,
       cro.[CleanedData] as [CRO],
       [NOMISNumber],
       pnc.[CleanedData] as [PNCNumber],
       [DateOfBirth],
       [GenderDescription],
       prison.[CleanedData] AS [PrisonNumber],
       'DELIUS' as SourceName
FROM [DeliusRunningPicture].[Offenders] o
    LEFT JOIN [DeliusRunningPicture].[StandardisationReference] frstn
        ON frstn.[RawData] = o.[FirstName]
           AND frstn.[Type] = 'CleanNameData'
    LEFT JOIN [DeliusRunningPicture].[StandardisationReference] secn
        ON secn.[RawData] = o.[SecondName]
           AND secn.[Type] = 'CleanNameData'
    LEFT JOIN [DeliusRunningPicture].[StandardisationReference] surn
        ON surn.[RawData] = o.[Surname]
           AND surn.[Type] = 'CleanNameData'
	LEFT JOIN [DeliusRunningPicture].[StandardisationReference] cro
        ON cro.[Key] = o.[CRN]
           AND cro.[RawData] = o.[CRO]
           AND cro.[Type] = 'CleanCROData'
	LEFT JOIN [DeliusRunningPicture].[StandardisationReference] prison
        ON cro.[Key] = o.[CRN]
           AND cro.[RawData] = o.[PrisonNumber]
           AND cro.[Type] = 'CleanPrisonNumberData'
	LEFT JOIN [DeliusRunningPicture].[StandardisationReference] pnc
        ON pnc.[Key] = o.[CRN]
           AND pnc.[RawData] = o.[PNCNumber]
           AND pnc.[Type] = 'CleanPNCData';