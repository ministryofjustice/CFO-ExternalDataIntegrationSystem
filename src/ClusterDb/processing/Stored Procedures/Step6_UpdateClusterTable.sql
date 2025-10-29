-- =============================================
-- Author:		Adam Bennett
-- Create date: 11/06/2024
-- Description: Given our cleaned-up graphs, update the cluster table
-- =============================================
CREATE PROCEDURE [processing].[Step6_UpdateClusterTable]
AS
BEGIN

    SET NOCOUNT ON;

    --Pull in any outstanding nodes
    INSERT INTO [output].ClusterMembership
    (
        NodeName,
        NodeKey,
        HardLink
    )
    SELECT n.NodeName,
           n.NodeKey,
           0
    FROM input.Nodes n
        LEFT OUTER JOIN [output].ClusterMembership c
            ON n.NodeName = c.NodeName
               AND n.NodeKey = c.NodeKey
    WHERE c.NodeName IS NULL;

    --Update cluster ids where appropriate - may involve creating new clusterids

    --TempClusterIds need mapping to the Permanent ClusterIds in ClusterMembership
    --We must be careful of cases where, based on the latest tempclusterids, a cluster has more than one clusterid previously
    --In this scenario we must keep only one clusterid - if any have a Hard Link then we must use that one
    --If a tempcluster has more than one hard link to different clusters then we have a problem
    --In this scenario we will not update the clusterid regardless

    DECLARE @SeedClusterId INT =
            (
                SELECT ISNULL(MAX(ClusterId), 0)FROM [output].ClusterMembership
            );


    TRUNCATE TABLE processing.LinkNodes;
    INSERT INTO processing.LinkNodes
    SELECT TempClusterId,
           LinkNodeName,
           LinkNodeKey
    FROM
    (
        SELECT TempClusterId,
               x.NodeName AS LinkNodeName,
               x.NodeKey AS LinkNodeKey,
               ROW_NUMBER() OVER (PARTITION BY TempClusterId
                                  ORDER BY CASE
                                               WHEN HardLink = 1 THEN
                                                   0
                                               WHEN ClusterId IS NOT NULL THEN
                                                   ClusterId
                                               ELSE
                                                   99999999
                                           END ASC
                                 ) AS i
        FROM
        (
            SELECT DISTINCT
                   cm.ClusterId,
                   cm.NodeName,
                   cm.NodeKey,
                   cm.HardLink,
                   ep.TempClusterId
            FROM [output].ClusterMembership cm
                INNER JOIN [output].Clusters_EdgeProbabilities ep
                    ON ep.SourceName = cm.NodeName
                       AND ep.SourceKey = cm.NodeKey
            UNION
            SELECT DISTINCT
                   cm.ClusterId,
                   cm.NodeName,
                   cm.NodeKey,
                   cm.HardLink,
                   ep.TempClusterId
            FROM [output].ClusterMembership cm
                INNER JOIN [output].Clusters_EdgeProbabilities ep
                    ON ep.TargetName = cm.NodeName
                       AND ep.TargetKey = cm.NodeKey
        ) x
    ) x
    WHERE i = 1;

    --Update where already existing ClusterId - these are cases where the clusterid has changed
    UPDATE cm
    SET cm.ClusterId = cml.ClusterId
    FROM [output].ClusterMembership cm
        INNER JOIN
        (
            SELECT DISTINCT
                   cm.ClusterId,
                   cm.NodeName,
                   cm.NodeKey,
                   cm.HardLink,
                   ep.TempClusterId
            FROM [output].ClusterMembership cm
                INNER JOIN [output].Clusters_EdgeProbabilities ep
                    ON ep.SourceName = cm.NodeName
                       AND ep.SourceKey = cm.NodeKey
            UNION
            SELECT DISTINCT
                   cm.ClusterId,
                   cm.NodeName,
                   cm.NodeKey,
                   cm.HardLink,
                   ep.TempClusterId
            FROM [output].ClusterMembership cm
                INNER JOIN [output].Clusters_EdgeProbabilities ep
                    ON ep.TargetName = cm.NodeName
                       AND ep.TargetKey = cm.NodeKey
        ) x
            ON cm.NodeName = x.NodeName
               AND cm.NodeKey = x.NodeKey
        INNER JOIN processing.LinkNodes ln
            ON x.TempClusterId = ln.TempClusterId
        INNER JOIN [output].ClusterMembership cml
            ON ln.LinkNodeName = cml.NodeName
               AND ln.LinkNodeKey = cml.NodeKey
    WHERE NOT (
                  cm.HardLink = 1
                  AND cm.ClusterId <> cml.ClusterId
              )
          AND cml.ClusterId IS NOT NULL
          AND cm.ClusterId <> cml.ClusterId;

    --Update where no existing clusterid - need to assign one
    UPDATE cm
    SET cm.ClusterId = x.ClusterId
    FROM [output].ClusterMembership cm
        INNER JOIN
        (
            SELECT cm.NodeName,
                   cm.NodeKey,
                   @SeedClusterId + DENSE_RANK() OVER (ORDER BY ln.TempClusterId) AS ClusterId
            FROM [output].ClusterMembership cm
                INNER JOIN
                (
                    SELECT DISTINCT
                           cm.ClusterId,
                           cm.NodeName,
                           cm.NodeKey,
                           cm.HardLink,
                           ep.TempClusterId
                    FROM [output].ClusterMembership cm
                        INNER JOIN [output].Clusters_EdgeProbabilities ep
                            ON ep.SourceName = cm.NodeName
                               AND ep.SourceKey = cm.NodeKey
                    UNION
                    SELECT DISTINCT
                           cm.ClusterId,
                           cm.NodeName,
                           cm.NodeKey,
                           cm.HardLink,
                           ep.TempClusterId
                    FROM [output].ClusterMembership cm
                        INNER JOIN [output].Clusters_EdgeProbabilities ep
                            ON ep.TargetName = cm.NodeName
                               AND ep.TargetKey = cm.NodeKey
                ) x
                    ON cm.NodeName = x.NodeName
                       AND cm.NodeKey = x.NodeKey
                INNER JOIN processing.LinkNodes ln
                    ON x.TempClusterId = ln.TempClusterId
                INNER JOIN [output].ClusterMembership cml
                    ON ln.LinkNodeName = cml.NodeName
                       AND ln.LinkNodeKey = cml.NodeKey
            WHERE NOT (
                          cm.HardLink = 1
                          AND cm.ClusterId <> cml.ClusterId
                      )
                  AND cml.ClusterId IS NULL
        ) x
            ON cm.NodeName = x.NodeName
               AND cm.NodeKey = x.NodeKey;

    --Any remaining null ClusterIds must be singlets - assign this a new clusterid
    SET @SeedClusterId =
    (
        SELECT ISNULL(MAX(ClusterId), 0)FROM [output].ClusterMembership
    );

    UPDATE cm
    SET cm.ClusterId = x.ClusterId
    FROM [output].ClusterMembership cm
        INNER JOIN
        (
            SELECT NodeName,
                   NodeKey,
                   @SeedClusterId + ROW_NUMBER() OVER (ORDER BY NodeName, NodeKey) AS ClusterId
            FROM [output].ClusterMembership
            WHERE ClusterId IS NULL
        ) x
            ON cm.NodeName = x.NodeName
               AND cm.NodeKey = x.NodeKey;


    --Update cluster membership probabilities
    --All singlets have a 100% probability of cluster membership
    --All other nodes will be given the mean square root of all edges to/from that node
    UPDATE cm
    SET cm.ClusterMembershipProbability = x.Probability
    FROM [output].ClusterMembership cm
        INNER JOIN
        (
            SELECT NodeName,
                   NodeKey,
                   AVG(SQRT(Probability)) AS Probability
            FROM
            (
                SELECT DISTINCT
                       cm.NodeName,
                       cm.NodeKey,
                       ep.SourceName,
                       ep.SourceKey,
                       ep.Probability
                FROM [output].ClusterMembership cm
                    INNER JOIN [output].Clusters_EdgeProbabilities ep
                        ON ep.SourceName = cm.NodeName
                           AND ep.SourceKey = cm.NodeKey
                UNION
                SELECT DISTINCT
                       cm.NodeName,
                       cm.NodeKey,
                       ep.TargetName,
                       ep.TargetKey,
                       ep.Probability
                FROM [output].ClusterMembership cm
                    INNER JOIN [output].Clusters_EdgeProbabilities ep
                        ON ep.TargetName = cm.NodeName
                           AND ep.TargetKey = cm.NodeKey
            ) x
            GROUP BY NodeName,
                     NodeKey
        ) x
            ON cm.NodeName = x.NodeName
               AND cm.NodeKey = x.NodeKey;

    UPDATE cm
    SET cm.ClusterMembershipProbability = 1
    FROM [output].ClusterMembership cm
        LEFT OUTER JOIN
        (
            SELECT NodeName,
                   NodeKey,
                   AVG(SQRT(Probability)) AS Probability
            FROM
            (
                SELECT DISTINCT
                       cm.NodeName,
                       cm.NodeKey,
                       ep.SourceName,
                       ep.SourceKey,
                       ep.Probability
                FROM [output].ClusterMembership cm
                    INNER JOIN [output].Clusters_EdgeProbabilities ep
                        ON ep.SourceName = cm.NodeName
                           AND ep.SourceKey = cm.NodeKey
                UNION
                SELECT DISTINCT
                       cm.NodeName,
                       cm.NodeKey,
                       ep.TargetName,
                       ep.TargetKey,
                       ep.Probability
                FROM [output].ClusterMembership cm
                    INNER JOIN [output].Clusters_EdgeProbabilities ep
                        ON ep.TargetName = cm.NodeName
                           AND ep.TargetKey = cm.NodeKey
            ) x
            GROUP BY NodeName,
                     NodeKey
        ) x
            ON cm.NodeName = x.NodeName
               AND cm.NodeKey = x.NodeKey
    WHERE x.NodeName IS NULL;


    --Clean up processing tables
    TRUNCATE TABLE processing.LinkNodes;


    --Add any outstanding clusters to output.Clusters
    INSERT INTO [output].Clusters
    (
        ClusterId,
        UPCI2
    )
    SELECT x.ClusterId,
           u.UPCI2
    FROM
    (
        SELECT ClusterId
        FROM [output].ClusterMembership cm
        GROUP BY ClusterId
    ) x
        LEFT OUTER JOIN [output].Clusters c
            ON c.ClusterId = x.ClusterId
        INNER JOIN reference.UPCI2 u
            ON x.ClusterId = u.ClusterId
    WHERE c.ClusterId IS NULL;


    --Update RecordCount
    --if not in ClusterMembersip, then count = 0 (empty cluster)
    UPDATE c
    SET c.RecordCount = ISNULL(cm.N, 0)
    FROM [output].Clusters c
        OUTER APPLY
    (
        SELECT COUNT(*) AS N
        FROM [output].ClusterMembership
        WHERE ClusterId = c.ClusterId
    ) cm;


    --Mark if has internal duplicates
    UPDATE c
    SET c.ContainsInternalDupe = CASE
                                     WHEN x.NodeName IS NULL THEN
                                         0
                                     ELSE
                                         1
                                 END
    FROM [output].Clusters c
        OUTER APPLY
    (
        SELECT TOP 1
               NodeName
        FROM [output].ClusterMembership
        WHERE ClusterId = c.ClusterId
        GROUP BY NodeName
        HAVING COUNT(*) > 1
    ) x;


    --Mark is contains low probability members
    UPDATE c
    SET c.ContainsLowProbabilityMembers = CASE
                                              WHEN x.ClusterId IS NULL THEN
                                                  0
                                              ELSE
                                                  1
                                          END
    FROM [output].Clusters c
        OUTER APPLY
    (
        SELECT TOP 1
               ClusterId
        FROM [output].ClusterMembership
        WHERE ClusterId = c.ClusterId
              AND ClusterMembershipProbability < 0.9
    ) x;


    --Review primary record
    --If primary record is no longer a member of the cluster then need to set a new primary record (or null if Record Count is 0)
    UPDATE c
    SET c.PrimaryRecordName = NULL,
        c.PrimaryRecordKey = NULL
    FROM [output].Clusters c
        LEFT OUTER JOIN [output].ClusterMembership cm
            ON c.PrimaryRecordName = cm.NodeName
               AND c.PrimaryRecordKey = cm.NodeKey
               AND c.ClusterId = cm.ClusterId
    WHERE c.PrimaryRecordName IS NOT NULL
          AND
          (
              cm.ClusterId IS NULL
              OR c.RecordCount = 0
          );

    --Only set if 1.) null or 2.) moving between prison and probation
    --If currently in prison then use Nomis else use Delius
    --If internal dupe then use the record with a hard link else the one with the highest memebership probability
    UPDATE c
    SET c.PrimaryRecordName = x.NodeName,
        c.PrimaryRecordKey = x.NodeKey
    FROM [output].Clusters c
        INNER JOIN
        (
            SELECT c.ClusterId,
                   cm.NodeName,
                   cm.NodeKey,
                   cm.HardLink,
                   ROW_NUMBER() OVER (PARTITION BY c.ClusterId
                                      ORDER BY CASE
                                                   WHEN o.NOMSnumber IS NOT NULL THEN
                                                       0
                                                   ELSE
                                                       1
                                               END ASC,
                                               CASE
                                                   WHEN cm.HardLink = 1 THEN
                                                       0
                                                   ELSE
                                                       1
                                               END ASC,
                                               cm.ClusterMembershipProbability DESC,
                                               cm.NodeName,
                                               cm.NodeKey
                                     ) AS i
            FROM [output].Clusters c
                LEFT OUTER JOIN [output].ClusterMembership cm
                    ON c.ClusterId = cm.ClusterId
                --**********************************************************************************************************************************************************************************************************
                LEFT OUTER JOIN
                (
                    SELECT NOMSnumber
                    FROM [$(OfflocRunningPictureDb)].OfflocRunningPicture.OffenderStatus
                    WHERE CustodyStatus LIKE 'Active%'
                ) o
                    ON cm.NodeName = 'NOMIS'
                       AND cm.NodeKey = o.NOMSnumber
        --**********************************************************************************************************************************************************************************************************
        ) x
            ON c.ClusterId = x.ClusterId
        LEFT OUTER JOIN [output].ClusterMembership cm
            ON c.ClusterId = cm.ClusterId
               AND c.PrimaryRecordName = cm.NodeName
               AND c.PrimaryRecordKey = cm.NodeKey
    WHERE x.i = 1
          AND
          (
              c.PrimaryRecordName IS NULL
              OR c.PrimaryRecordName <> x.NodeName
              OR x.HardLink = 1
                 AND x.HardLink = 0
          );

END;