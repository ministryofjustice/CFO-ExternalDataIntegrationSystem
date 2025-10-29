CREATE VIEW [OfflocRunningPicture].[RecordMatchingView]
AS
SELECT [PersonalDetails].[NOMSnumber],
       [cro].[CROno],
       [pnc].[Details] as [PNC],
       [PersonalDetails].[Forename1],
       [PersonalDetails].[Forename2],
       [PersonalDetails].[Surname],
       [PersonalDetails].[DOB],
       [PersonalDetails].[Gender],
	   'NOMIS' as [SourceName]
FROM [OfflocRunningPicture].[PersonalDetailsCleanView]  as [PersonalDetails]
    LEFT JOIN
    (
        SELECT i.NOMSnumber,
               i.CROno,
               ROW_NUMBER() OVER (PARTITION BY i.NOMSnumber ORDER BY i.IsActive DESC, i.ValidFrom DESC, i.CROno) AS idx
        FROM OfflocRunningPicture.Identifiers AS i
    ) AS cro
        ON cro.NOMSnumber = [PersonalDetails].NOMSnumber
           AND cro.idx = 1
    LEFT JOIN
    (
        SELECT i.NOMSnumber,
               i.Details,
               ROW_NUMBER() OVER (PARTITION BY i.NOMSnumber ORDER BY i.IsActive DESC, i.ValidFrom DESC, i.Details) AS idx
        FROM OfflocRunningPicture.PNC AS i
    ) AS pnc
        ON pnc.NOMSnumber = [PersonalDetails].NOMSnumber
           AND pnc.idx = 1;
GO