

-- =============================================
-- Author:		Adam Bennett
-- Create date: 11/06/2024
-- =============================================
CREATE PROCEDURE [processing].[Step3_GetPass2Edges]
AS
BEGIN

    SET NOCOUNT ON;


    --Add the outstanding edges to ClusterAssignment, complete with their probabilities

    INSERT INTO processing.ClusterAssignment
    SELECT oe.SourceName,
           oe.SourceKey,
           oe.TargetName,
           oe.TargetKey,
           MAX(ep.Probability),
           oe.TempClusterId
    FROM processing.OutstandingEdges oe
        INNER JOIN input.EdgeProbabilities_Pass2 ep
            ON oe.SourceName = ep.SourceName
               AND oe.SourceKey = ep.SourceKey
               AND oe.TargetName = ep.TargetName
               AND oe.TargetKey = ep.TargetKey
    GROUP BY oe.SourceName,
             oe.SourceKey,
             oe.TargetName,
             oe.TargetKey,
             oe.TempClusterId;

    --We can now clean-up the input tables - they are no longer required
    TRUNCATE TABLE input.EdgeProbabilities_Pass1;
    TRUNCATE TABLE input.EdgeProbabilities_Pass2;


END;