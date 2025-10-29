
-- =============================================
-- Author:		Adam Bennett
-- Create date: 11/06/2024
-- Description: Maintain master list of all records ever seen in the Nodes table
-- =============================================
CREATE PROCEDURE [processing].[Step5_MaintainNodeTable]
AS
BEGIN

    SET NOCOUNT ON;

    --input.Nodes should contain a unique list of all the records in Delius, Nomis and any other system being matched to a cluster / UPCI
    --Ideally this can be pushed to the ClusterDB by the stagers when data is imported
    --but here we will pull the data in to ensure nothing is missed.
    --Any record pairs provided by the matcher must only contain nodes that are in input.Nodes,
    --however it is appreciated that input.Nodes may not be up-to-date when the ClusterDB is called.
    --However, input.Nodes ***MUST*** be up-to-date from this point.

    --Pull in any outstanding nodes from the ClusterDB
    INSERT INTO input.Nodes
    SELECT x.SourceName,
           x.SourceKey
    FROM
    (
        SELECT DISTINCT
               SourceName,
               SourceKey
        FROM [output].Clusters_EdgeProbabilities
        UNION
        SELECT DISTINCT
               TargetName,
               TargetKey
        FROM [output].Clusters_EdgeProbabilities
    ) x
        LEFT OUTER JOIN input.Nodes n
            ON x.SourceName = n.NodeName
               AND x.SourceKey = n.NodeKey
    WHERE n.NodeName IS NULL;

    --Pull in any outstanding nodes from Delius
    INSERT INTO input.Nodes
    SELECT DISTINCT
           'DELIUS',
           x.CRN
    FROM [$(DeliusRunningPictureDb)].DeliusRunningPicture.Offenders x
        LEFT OUTER JOIN input.Nodes n
            ON n.NodeName = 'DELIUS'
               AND x.CRN = n.NodeKey
    WHERE n.NodeName IS NULL;

    --Pull in any outstanding nodes from Offloc
    INSERT INTO input.Nodes
    SELECT DISTINCT
           'NOMIS',
           x.NOMSnumber
    FROM [$(OfflocRunningPictureDb)].OfflocRunningPicture.Bookings x
        LEFT OUTER JOIN input.Nodes n
            ON n.NodeName = 'NOMIS'
               AND x.NOMSnumber = n.NodeKey
    WHERE n.NodeName IS NULL;

END;