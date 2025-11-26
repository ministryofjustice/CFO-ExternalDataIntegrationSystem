

CREATE VIEW [Matching].[ListMatches]
AS
SELECT M.CandidateId,
       C.l_SourceName AS [l_Source],
       C.r_SourceName AS [r_Source],
       M.Probability,
       C.l_FirstName,
       C.l_SecondName,
       C.l_LastName,
       C.r_FirstName,
       C.r_SecondName,
       C.r_LastName,
       C.l_DateOfBirth,
       C.r_DateOfBirth,
       C.l_NOMISNumber,
       C.r_NOMISNumber,
       C.l_CRONumber,
       C.r_CRONumber,
       C.l_Gender,
       C.r_Gender,
       M.JSON
FROM Matching.Matches M
    INNER JOIN Matching.Candidates C
        ON M.CandidateId = C.CandidateId;
