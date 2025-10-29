CREATE VIEW [OfflocRunningPicture].[PersonalDetailsCleanView]
AS
SELECT DISTINCT
       pd.NOMSnumber,
       LOWER(COALESCE(fn1.CleanedData, pd.Forename1)) AS ForeName1,
       LOWER(COALESCE(fn2.CleanedData, pd.Forename2)) AS ForeName2,
       LOWER(COALESCE(sn.CleanedData, pd.Surname)) AS Surname,
       pd.DOB,
       pd.Gender,
       'NOMIS' as SourceName
FROM OfflocRunningPicture.PersonalDetails AS pd
    LEFT OUTER JOIN OfflocRunningPicture.StandardisationReference AS fn1
        ON fn1.RawData = pd.Forename1
           AND fn1.Type = 'CleanNameData'
    LEFT OUTER JOIN OfflocRunningPicture.StandardisationReference AS fn2
        ON fn2.RawData = pd.Forename2
           AND fn2.Type = 'CleanNameData'
    LEFT OUTER JOIN OfflocRunningPicture.StandardisationReference AS sn
        ON sn.RawData = pd.Surname
           AND sn.Type = 'CleanNameData';