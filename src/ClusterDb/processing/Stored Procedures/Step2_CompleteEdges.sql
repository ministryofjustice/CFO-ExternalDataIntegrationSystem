-- =============================================
-- Author:		Adam Bennett
-- Create date: 11/06/2024
-- Description: Given the output of MatchingDB determines if any edges are outstanding and gets them ready for the matcher
-- =============================================
CREATE PROCEDURE [processing].[Step2_CompleteEdges]
AS
BEGIN

    SET NOCOUNT ON;

    --Seed clusters
    --We ensure that each edge is unique (as our graph are non-directional we will my convention assign the 2 nodes to A and B alphabetically)
    --We only include probable links at this stage
    TRUNCATE TABLE processing.ClusterAssignment;
    INSERT INTO processing.ClusterAssignment
    SELECT SourceName,
           SourceKey,
           TargetName,
           TargetKey,
           Probability,
           ROW_NUMBER() OVER (ORDER BY SourceName, SourceKey, TargetName, TargetKey) AS ClusterId
    FROM
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
               END AS TargetKey,
               MAX(Probability) AS Probability
        FROM input.EdgeProbabilities_Pass1
        WHERE SourceName <> TargetName
              AND SourceKey <> TargetKey
        GROUP BY SourceName,
                 SourceKey,
                 TargetName,
                 TargetKey
    ) x
    WHERE Probability >= 0.5;


    --Quickly remove any clusters consisting of a single pair of nodes (this is expected to be most clusters)
    --Add them to the output table for later (only retain edges with valid links)
    TRUNCATE TABLE [output].[Clusters_EdgeProbabilities];
    INSERT INTO [output].[Clusters_EdgeProbabilities]
    SELECT ROW_NUMBER() OVER (ORDER BY x.SourceName, x.SourceKey, x.TargetName, x.TargetKey) AS ClusterId,
           x.SourceName,
           x.SourceKey,
           x.TargetName,
           x.TargetKey,
           x.Probability
    FROM processing.ClusterAssignment x
        LEFT OUTER JOIN processing.ClusterAssignment a
            ON x.SourceName = a.SourceName
               AND x.SourceKey = a.SourceKey
               AND NOT (
                           x.SourceName = a.SourceName
                           AND x.SourceKey = a.SourceKey
                           AND x.TargetName = a.TargetName
                           AND x.TargetKey = a.TargetKey
                       )
        LEFT OUTER JOIN processing.ClusterAssignment b
            ON x.SourceName = b.TargetName
               AND x.SourceKey = b.TargetKey
               AND NOT (
                           x.SourceName = b.SourceName
                           AND x.SourceKey = b.SourceKey
                           AND x.TargetName = b.TargetName
                           AND x.TargetKey = b.TargetKey
                       )
        LEFT OUTER JOIN processing.ClusterAssignment c
            ON x.TargetName = c.SourceName
               AND x.TargetKey = c.SourceKey
               AND NOT (
                           x.SourceName = c.SourceName
                           AND x.SourceKey = c.SourceKey
                           AND x.TargetName = c.TargetName
                           AND x.TargetKey = c.TargetKey
                       )
        LEFT OUTER JOIN processing.ClusterAssignment d
            ON x.TargetName = d.TargetName
               AND x.TargetKey = d.TargetKey
               AND NOT (
                           x.SourceName = d.SourceName
                           AND x.SourceKey = d.SourceKey
                           AND x.TargetName = d.TargetName
                           AND x.TargetKey = d.TargetKey
                       )
    WHERE a.SourceName IS NULL
          AND b.SourceName IS NULL
          AND c.SourceName IS NULL
          AND d.SourceName IS NULL
          AND x.Probability >= 0.5;

    DELETE x
    FROM processing.ClusterAssignment x
        LEFT OUTER JOIN processing.ClusterAssignment a
            ON x.SourceName = a.SourceName
               AND x.SourceKey = a.SourceKey
               AND NOT (
                           x.SourceName = a.SourceName
                           AND x.SourceKey = a.SourceKey
                           AND x.TargetName = a.TargetName
                           AND x.TargetKey = a.TargetKey
                       )
        LEFT OUTER JOIN processing.ClusterAssignment b
            ON x.SourceName = b.TargetName
               AND x.SourceKey = b.TargetKey
               AND NOT (
                           x.SourceName = b.SourceName
                           AND x.SourceKey = b.SourceKey
                           AND x.TargetName = b.TargetName
                           AND x.TargetKey = b.TargetKey
                       )
        LEFT OUTER JOIN processing.ClusterAssignment c
            ON x.TargetName = c.SourceName
               AND x.TargetKey = c.SourceKey
               AND NOT (
                           x.SourceName = c.SourceName
                           AND x.SourceKey = c.SourceKey
                           AND x.TargetName = c.TargetName
                           AND x.TargetKey = c.TargetKey
                       )
        LEFT OUTER JOIN processing.ClusterAssignment d
            ON x.TargetName = d.TargetName
               AND x.TargetKey = d.TargetKey
               AND NOT (
                           x.SourceName = d.SourceName
                           AND x.SourceKey = d.SourceKey
                           AND x.TargetName = d.TargetName
                           AND x.TargetKey = d.TargetKey
                       )
    WHERE a.SourceName IS NULL
          AND b.SourceName IS NULL
          AND c.SourceName IS NULL
          AND d.SourceName IS NULL;


    --Enumerate over edges to complete initial cluster assignment
    --Once complete, no cluster should have a link to another cluster
    WHILE @@ROWCOUNT > 0
    BEGIN
        UPDATE y
        SET y.TempClusterId = x.TempClusterId
        FROM processing.ClusterAssignment x
            INNER JOIN processing.ClusterAssignment y
                ON (
                       (
                           x.SourceName = y.SourceName
                           AND x.SourceKey = y.SourceKey
                       )
                       OR
                       (
                           x.SourceName = y.TargetName
                           AND x.SourceKey = y.TargetKey
                       )
                       OR
                       (
                           x.TargetName = y.SourceName
                           AND x.TargetKey = y.SourceKey
                       )
                       OR
                       (
                           x.TargetName = y.TargetName
                           AND x.TargetKey = y.TargetKey
                       )
                   )
                   AND x.TempClusterId < y.TempClusterId;
    END;


    --We can resolve clusters for graphs with a node count of 5 or less
    --Larger graphs are too expensive to resolve precisely. Instead these need to be crudely decompossed into smaller graphs within this threshold, which in turn can then be resolved fully
    --For each cluster we prune the weakest link and reassign clusterids, repeating until no clusters have more than 5 nodes
    DECLARE @MaxNodeCount INT = 5,
            @lastupdated INT = 1;


    WHILE @lastupdated > 0
    BEGIN
        --Prune
        DELETE a
        FROM processing.ClusterAssignment a
            INNER JOIN
            (
                SELECT SourceName,
                       SourceKey,
                       TargetName,
                       TargetKey
                FROM
                (
                    SELECT c.SourceName,
                           c.SourceKey,
                           c.TargetName,
                           c.TargetKey,
                           ROW_NUMBER() OVER (PARTITION BY c.TempClusterId ORDER BY c.Probability ASC) AS i
                    FROM
                    (
                        SELECT TempClusterId
                        FROM
                        (
                            SELECT DISTINCT
                                   TempClusterId,
                                   SourceName,
                                   SourceKey
                            FROM processing.ClusterAssignment
                            UNION
                            SELECT DISTINCT
                                   TempClusterId,
                                   TargetName,
                                   TargetKey
                            FROM processing.ClusterAssignment
                        ) x
                        GROUP BY TempClusterId
                        HAVING COUNT(*) > @MaxNodeCount
                    ) x
                        INNER JOIN processing.ClusterAssignment c
                            ON x.TempClusterId = c.TempClusterId
                ) x
                WHERE i = 1
            ) b
                ON a.SourceName = b.SourceName
                   AND a.SourceKey = b.SourceKey
                   AND a.TargetName = b.TargetName
                   AND a.TargetKey = b.TargetKey;

        SET @lastupdated = @@ROWCOUNT;

        --Reassign clusters
        WHILE @@ROWCOUNT > 0
        BEGIN
            UPDATE y
            SET y.TempClusterId = x.TempClusterId
            FROM processing.ClusterAssignment x
                INNER JOIN processing.ClusterAssignment y
                    ON (
                           (
                               x.SourceName = y.SourceName
                               AND x.SourceKey = y.SourceKey
                           )
                           OR
                           (
                               x.SourceName = y.TargetName
                               AND x.SourceKey = y.TargetKey
                           )
                           OR
                           (
                               x.TargetName = y.SourceName
                               AND x.TargetKey = y.SourceKey
                           )
                           OR
                           (
                               x.TargetName = y.TargetName
                               AND x.TargetKey = y.TargetKey
                           )
                       )
                       AND x.TempClusterId < y.TempClusterId;
        END;
    END;

    --We need to have the probabilities for all edges within a graph (a complete graph)
    --Determine which edges are outstanding - these need sending back to the scorer to determine their probabilities
    --At the end of this step all of our graphs with be complete - all edges will have known probabilities

    --Store cluster meta data
    TRUNCATE TABLE processing.ClusterMeta;
    INSERT INTO processing.ClusterMeta
    SELECT TempClusterId,
           SourceName AS NodeName,
           SourceKey AS NodeKey,
           ROW_NUMBER() OVER (PARTITION BY TempClusterId ORDER BY SourceName, SourceKey) AS NodeId,
           COUNT(*) OVER (PARTITION BY TempClusterId) AS NodeCount
    FROM
    (
        SELECT DISTINCT
               TempClusterId,
               SourceName,
               SourceKey
        FROM processing.ClusterAssignment
        UNION
        SELECT DISTINCT
               TempClusterId,
               TargetName,
               TargetKey
        FROM processing.ClusterAssignment
    ) x;


    --Store outstanding edges
    TRUNCATE TABLE processing.OutstandingEdges;
    INSERT INTO processing.OutstandingEdges
    SELECT x.TempClusterId,
           CASE
               WHEN mA.NodeSource + mA.NodeKey < mB.NodeSource + mB.NodeKey THEN
                   mA.NodeSource
               ELSE
                   mB.NodeSource
           END AS SourceName,
           CASE
               WHEN mA.NodeSource + mA.NodeKey < mB.NodeSource + mB.NodeKey THEN
                   mA.NodeKey
               ELSE
                   mB.NodeKey
           END AS SourceKey,
           CASE
               WHEN mA.NodeSource + mA.NodeKey < mB.NodeSource + mB.NodeKey THEN
                   mB.NodeSource
               ELSE
                   mA.NodeSource
           END AS TargetName,
           CASE
               WHEN mA.NodeSource + mA.NodeKey < mB.NodeSource + mB.NodeKey THEN
                   mB.NodeKey
               ELSE
                   mA.NodeKey
           END AS TargetKey
    FROM
    (
        SELECT e.TempClusterId,
               e.NodeIdA,
               e.NodeIdB
        FROM
        (
            SELECT m.TempClusterId,
                   e.NodeIdA,
                   e.NodeIdB
            FROM reference.Edges e
                INNER JOIN
                (SELECT DISTINCT TempClusterId, NodeCount FROM processing.ClusterMeta) m
                    ON e.NodeCount = m.NodeCount
        ) e
            LEFT OUTER JOIN
            (
                SELECT c.TempClusterId,
                       mA.NodeCount,
                       mA.NodeId AS NodeIdA,
                       mB.NodeId AS NodeIdB
                FROM processing.ClusterAssignment c
                    INNER JOIN processing.ClusterMeta mA
                        ON c.SourceName = mA.NodeSource
                           AND c.SourceKey = mA.NodeKey
                    INNER JOIN processing.ClusterMeta mB
                        ON c.TargetName = mB.NodeSource
                           AND c.TargetKey = mB.NodeKey
            ) x
                ON e.TempClusterId = x.TempClusterId
                   AND e.NodeIdA = x.NodeIdA
                   AND e.NodeIdB = x.NodeIdB
        WHERE x.TempClusterId IS NULL
    ) x
        INNER JOIN processing.ClusterMeta mA
            ON x.TempClusterId = mA.TempClusterId
               AND x.NodeIdA = mA.NodeId
        INNER JOIN processing.ClusterMeta mB
            ON x.TempClusterId = mB.TempClusterId
               AND x.NodeIdB = mB.NodeId;

    --Wipe down pass 2 edge probability table so ready for the matcher
    TRUNCATE TABLE input.EdgeProbabilities_Pass2;

    --*******************************************************************************************
    --We must now go and get the matcher to determine the probabilities for the outstanding edges
    --*******************************************************************************************

END;