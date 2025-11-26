-- =============================================
-- Author:		Adam Bennett
-- Create date: 12/06/2024
-- Description:	Flattens all tables (except reference tables)
-- =============================================
CREATE PROCEDURE [processing].[FlattenTables]
AS
BEGIN

    SET NOCOUNT ON;

    TRUNCATE TABLE input.EdgeProbabilities_Pass1;
    TRUNCATE TABLE input.EdgeProbabilities_Pass2;
    TRUNCATE TABLE input.Nodes;
    TRUNCATE TABLE [output].ClusterMembership;
    TRUNCATE TABLE [output].Clusters;
    TRUNCATE TABLE [output].Clusters_EdgeProbabilities;
    TRUNCATE TABLE processing.ClusterAssignment;
    TRUNCATE TABLE processing.ClusterAssignment2;
    TRUNCATE TABLE processing.ClusterMeta;
    TRUNCATE TABLE processing.LinkNodes;
    TRUNCATE TABLE processing.OutstandingEdges;
    TRUNCATE TABLE processing.SubGraphs;
    TRUNCATE TABLE search.ClusterAttributes;

END;