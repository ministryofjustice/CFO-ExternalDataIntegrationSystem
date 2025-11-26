
-- =============================================
-- Author:		Adam Bennett
-- Create date: 11/06/2024
-- =============================================
CREATE PROCEDURE [processing].[Step1_GetPass1Edges]
AS
BEGIN

    SET NOCOUNT ON;

    --******Updated with latest schema 12/06/2024******

    --Input initial edges from the MatchingDB into input.EdgeProbabilities_Pass1
    --Assumed that each edge is unique
    TRUNCATE TABLE input.EdgeProbabilities_Pass1;
	INSERT INTO input.EdgeProbabilities_Pass1
	(
		SourceName,
		SourceKey,
		TargetName,
		TargetKey,
		Probability
	)
	SELECT c.l_SourceName,
		   c.l_SourceKey,
		   c.r_SourceName,
		   c.r_SourceKey,
		   MAX(m.Probability) AS Probability
	FROM MatchingDb.Matching.Candidates c
		INNER JOIN MatchingDb.Matching.Matches m
			ON c.CandidateId = m.CandidateId
	GROUP BY c.l_SourceKey,
			 c.l_SourceName,
			 c.r_SourceKey,
			 c.r_SourceName;

    --Add any outstanding edges from existing clusters
    INSERT INTO input.EdgeProbabilities_Pass1
    SELECT ep.SourceName,
           ep.SourceKey,
           ep.TargetName,
           ep.TargetKey,
           ep.Probability
    FROM [output].Clusters_EdgeProbabilities ep
        LEFT OUTER JOIN
        (
            SELECT CASE
                       WHEN SourceName + SourceKey < TargetName + TargetKey THEN
                           SourceName
                       ELSE
                           TargetName
                   END AS SourceName,
                   CASE
                       WHEN SourceName + SourceKey < TargetName + TargetKey THEN
                           SourceKey
                       ELSE
                           TargetKey
                   END AS SourceKey,
                   CASE
                       WHEN SourceName + SourceKey < TargetName + TargetKey THEN
                           TargetName
                       ELSE
                           SourceName
                   END AS TargetName,
                   CASE
                       WHEN SourceName + SourceKey < TargetName + TargetKey THEN
                           TargetKey
                       ELSE
                           SourceKey
                   END AS TargetKey
            FROM input.EdgeProbabilities_Pass1
            WHERE SourceName <> TargetName
                  AND SourceKey <> TargetKey
            GROUP BY SourceName,
                     SourceKey,
                     TargetName,
                     TargetKey
        ) x
            ON ep.SourceName = x.SourceName
               AND ep.SourceKey = x.SourceKey
               AND ep.TargetName = x.TargetName
               AND ep.TargetKey = x.TargetKey
    WHERE x.SourceName IS NULL;

END;