

CREATE   VIEW [Clustering].[RecordMatchingView]
AS
WITH Candidates_CTE (SourceName, SourceKey, NOMISNumber, PNCNumber, FirstName, SecondName, LastName, DateOfBirth,
                     CRONumber, Gender
                    )
AS (
   SELECT l_SourceName AS SourceName,
          l_SourceKey AS SourceKey,
          l_NOMISNumber AS NOMISNumber,
          l_PNCNumber AS PNCNumber,
          l_FirstName AS FirstName,
          l_SecondName AS SecondName,
          l_LastName AS LastName,
          l_DateOfBirth AS DateOfBirth,
          l_CRONumber AS CRONumber,
          l_Gender AS Gender
   FROM [$(MatchingDb)].Matching.Candidates C
   UNION
   SELECT r_SourceName AS SourceName,
          r_SourceKey AS SourceKey,
          r_NOMISNumber AS NOMISNumber,
          r_PNCNumber AS PNCNumber,
          r_FirstName AS FirstName,
          r_SecondName AS SecondName,
          r_LastName AS LastName,
          r_DateOfBirth AS DateOfBirth,
          r_CRONumber AS CRONumber,
          r_Gender AS Gender
   FROM [$(MatchingDb)].Matching.Candidates)
SELECT C1.SourceName AS l_SourceName,
       C1.SourceKey AS l_SourceKey,
       C1.NOMISNumber AS l_NOMISNumber,
       C1.PNCNumber AS l_PNCNumber,
       C1.FirstName AS l_FirstName,
       C1.SecondName AS l_SecondName,
       C1.LastName AS l_LastName,
       C1.DateOfBirth AS l_DateOfBirth,
       C1.CRONumber AS l_CRONumber,
       C1.Gender AS l_Gender,
       C2.SourceName AS r_SourceName,
       C2.SourceKey AS r_SourceKey,
       C2.NOMISNumber AS r_NOMISNumber,
       C2.PNCNumber AS r_PNCNumber,
       C2.FirstName AS r_FirstName,
       C2.SecondName AS r_SecondName,
       C2.LastName AS r_LastName,
       C2.DateOfBirth AS r_DateOfBirth,
       C2.CRONumber AS r_CRONumber,
       C2.Gender AS r_Gender
FROM processing.OutstandingEdges O
    INNER JOIN Candidates_CTE C1
        ON C1.SourceKey = O.SourceKey
           AND C1.SourceName = O.SourceName
    INNER JOIN Candidates_CTE C2
        ON C2.SourceKey = O.TargetKey
           AND C2.SourceName = O.TargetName;