-- =============================================
-- Author:		Adam Bennett
-- Create date: 11/06/2024
-- Description: Given a all graphs are now complete, resolve into the most probable subgraphs given our algorithm
-- =============================================
CREATE PROCEDURE [processing].[Step4_ResolveGraphs]
AS
BEGIN

    SET NOCOUNT ON;

    --All of our clusters are now complete graphs (the probability of each edge is known)
    --For each graph:

    --Determine all of the possible sets of subgraphs subject to the following constraints:
    --1. All subgraphs are complete
    --2. No node is included more than once in the set
    --3. All nodes in the graph must be included
    --For speed/ease these are already predetermined and held in reference.subGraphSets
    --For each set of subgraphs calculate the probability of that set
    --Return the most probable set of subgraphs

    TRUNCATE TABLE processing.SubGraphs;
    INSERT INTO processing.SubGraphs
    SELECT TempClusterId,
           NodeCount,
           SubGraphSetId
    FROM
    (
        SELECT TempClusterId,
               NodeCount,
               SubGraphSetId,
               ROW_NUMBER() OVER (PARTITION BY TempClusterId ORDER BY Probability DESC) AS i
        FROM
        (
            SELECT c.TempClusterId,
                   s.NodeCount,
                   s.SubGraphSetId,
                   EXP(SUM(LOG(   CASE
                                      WHEN s.KeepEdge = 1 THEN
                                          c.Probability
                                      ELSE
                                          1 - c.Probability
                                  END
                              )
                          )
                      ) AS Probability
            FROM processing.ClusterAssignment c
                INNER JOIN processing.ClusterMeta mA
                    ON c.SourceName = mA.NodeSource
                       AND c.SourceKey = mA.NodeKey
                INNER JOIN processing.ClusterMeta mB
                    ON c.TargetName = mB.NodeSource
                       AND c.TargetKey = mB.NodeKey
                INNER JOIN reference.SubGraphSets s
                    ON mA.NodeCount = s.NodeCount
                       AND mA.NodeId = s.NodeIdA
                       AND mB.NodeId = s.NodeIdB
            GROUP BY c.TempClusterId,
                     s.NodeCount,
                     s.SubGraphSetId
        ) x
    ) x
    WHERE i = 1;


    --Given the most probable set of subgraphs determine which edges need to be retained
    --Populate ClusterAssignment2 table and enumerate over to determine new clusterids
    TRUNCATE TABLE processing.ClusterAssignment2;
    INSERT INTO processing.ClusterAssignment2
    SELECT mA.NodeSource AS SourceName,
           mA.NodeKey AS SourceKey,
           mB.NodeSource AS TargetName,
           mB.NodeKey AS TargetKey,
           c.Probability,
           ROW_NUMBER() OVER (ORDER BY mA.NodeSource, mA.NodeKey, mB.NodeSource, mB.NodeKey) AS ClusterId
    FROM processing.SubGraphs s
        INNER JOIN reference.SubGraphSets r
            ON s.Nodecount = r.NodeCount
               AND s.SubGraphSetId = r.SubGraphSetId
        INNER JOIN processing.ClusterMeta mA
            ON r.NodeIdA = mA.NodeId
               AND mA.TempClusterId = s.TempClusterId
        INNER JOIN processing.ClusterMeta mB
            ON r.NodeIdB = mB.NodeId
               AND mB.TempClusterId = s.TempClusterId
        INNER JOIN processing.ClusterAssignment c
            ON c.TempClusterId = s.TempClusterId
               AND c.SourceName = mA.NodeSource
               AND c.SourceKey = mA.NodeKey
               AND c.TargetName = mB.NodeSource
               AND c.TargetKey = mB.NodeKey
    WHERE r.KeepEdge = 1;

    WHILE @@ROWCOUNT > 0
    BEGIN
        UPDATE y
        SET y.TempClusterId = x.TempClusterId
        FROM processing.ClusterAssignment2 x
            INNER JOIN processing.ClusterAssignment2 y
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

    --Add our resolved clusters to the output table with new clusterIds
    INSERT INTO [output].Clusters_EdgeProbabilities
    SELECT DENSE_RANK() OVER (ORDER BY TempClusterId) +
           (
               SELECT MAX(TempClusterId)FROM [output].Clusters_EdgeProbabilities
           ) AS ClusterId,
           SourceName,
           SourceKey,
           TargetName,
           TargetKey,
           Probability
    FROM processing.ClusterAssignment2;


    --The output table Clusters_EdgeProbabilities now contains the most probable set of valid clusters based on the input tables
    --The output table may not contain all nodes from the input table as they may have been pruned off into 'singlets'

    --Clean up processing tables
    TRUNCATE TABLE processing.ClusterAssignment;
    TRUNCATE TABLE processing.ClusterAssignment2;
    TRUNCATE TABLE processing.ClusterMeta;
    TRUNCATE TABLE processing.OutstandingEdges;
    TRUNCATE TABLE processing.SubGraphs;

END;