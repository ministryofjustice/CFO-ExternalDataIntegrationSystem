
CREATE   VIEW [Matching].[ListBlockedCandidates]
AS
SELECT B.BlockId AS BlockId,
       L.SourceName AS l_SourceName,
       L.NOMSnumber AS l_SourceKey,
       L.NOMSnumber AS l_NOMISNumber,
       L.PNC AS l_PNCNumber,
       L.Forename1 AS l_FirstName,
       L.Forename2 AS l_SecondName,
       L.Surname AS l_LastName,
       L.DOB AS l_DateOfBirth,
       L.CROno AS l_CRONumber,
       L.Gender AS l_Gender,
       R.SourceName AS r_SourceName,
       R.CRN AS r_SourceKey,
       R.PNCNumber AS r_PNCNumber,
       R.FirstName AS r_FirstName,
       R.SecondName AS r_SecondName,
       R.Surname AS r_LastName,
       R.DateOfBirth AS r_DateOfBirth,
       R.NOMISNumber AS r_NOMISNumber,
       R.CRO AS r_CRONumber,
       R.GenderDescription AS r_Gender
FROM Matching.Blocks AS B
    INNER JOIN [$(OfflocRunningPictureDb)].OfflocRunningPicture.RecordMatchingView AS L
        ON B.SourceKey = L.NOMSnumber
           AND L.[SourceName] = 'NOMIS'
    INNER JOIN [$(DeliusRunningPictureDb)].DeliusRunningPicture.RecordMatchingView AS R
        ON B.TargetKey = R.CRN
           AND R.[SourceName] = 'DELIUS'
UNION
SELECT B.BlockId AS BlockId,
       L.SourceName AS l_SourceName,
       L.NOMSnumber AS l_SourceKey,
       L.NOMSnumber AS l_NOMISNumber,
       L.PNC AS l_PNCNumber,
       L.Forename1 AS l_FirstName,
       L.Forename2 AS l_SecondName,
       L.Surname AS l_LastName,
       L.DOB AS l_DateOfBirth,
       L.CROno AS l_CRONumber,
       L.Gender AS l_Gender,
       R.SourceName AS r_SourceName,
       R.NOMSnumber AS r_SourceKey,
       R.PNC AS r_PNCNumber,
       R.Forename1 AS r_FirstName,
       R.Forename2 AS r_SecondName,
       R.Surname AS r_LastName,
       R.DOB AS r_DateOfBirth,
       R.NOMSnumber AS r_NOMISNumber,
       R.CROno AS r_CRONumber,
       R.Gender AS r_Gender
FROM Matching.Blocks AS B
    INNER JOIN [$(OfflocRunningPictureDb)].OfflocRunningPicture.RecordMatchingView AS L
        ON B.SourceKey = L.NOMSnumber
           AND L.[SourceName] = 'NOMIS'
    INNER JOIN [$(OfflocRunningPictureDb)].OfflocRunningPicture.RecordMatchingView AS R
        ON B.TargetKey = R.NOMSnumber
           AND R.[SourceName] = 'NOMIS'
UNION
SELECT B.BlockId AS BlockId,
       L.SourceName AS l_SourceName,
       L.CRN AS l_SourceKey,
       L.NOMISNumber AS l_NOMISNumber,
       L.PNCNumber AS l_PNCNumber,
       L.FirstName AS l_FirstName,
       L.SecondName AS l_SecondName,
       L.Surname AS l_LastName,
       L.DateOfBirth AS l_DateOfBirth,
       L.CRO AS l_CRONumber,
       L.GenderDescription AS l_Gender,
       R.SourceName AS r_SourceName,
       R.CRN AS r_SourceKey,
       R.PNCNumber AS r_PNCNumber,
       R.FirstName AS r_FirstName,
       R.SecondName AS r_SecondName,
       R.Surname AS r_LastName,
       R.DateOfBirth AS r_DateOfBirth,
       R.NOMISNumber AS r_NOMISNumber,
       R.CRO AS r_CRONumber,
       R.GenderDescription AS r_Gender
FROM Matching.Blocks AS B
    INNER JOIN [$(DeliusRunningPictureDb)].DeliusRunningPicture.RecordMatchingView AS L
        ON B.SourceKey = L.CRN
           AND L.[SourceName] = 'DELIUS'
    INNER JOIN [$(DeliusRunningPictureDb)].DeliusRunningPicture.RecordMatchingView AS R
        ON B.TargetKey = R.CRN
           AND R.[SourceName] = 'DELIUS';
