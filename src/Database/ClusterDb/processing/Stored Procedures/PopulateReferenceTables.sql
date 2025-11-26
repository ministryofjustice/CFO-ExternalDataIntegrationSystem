-- =============================================
-- Author:		Adam Bennett
-- Create date: 17/06/2024
-- Description:	Populates reference tables
-- =============================================
CREATE   PROCEDURE [processing].[PopulateReferenceTables]
AS
BEGIN

	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM reference.Nodes)
	BEGIN
		Insert into reference.Nodes (NodeCount, NodeId) Select 3,1
		Insert into reference.Nodes (NodeCount, NodeId) Select 3,2
		Insert into reference.Nodes (NodeCount, NodeId) Select 3,3
		Insert into reference.Nodes (NodeCount, NodeId) Select 4,1
		Insert into reference.Nodes (NodeCount, NodeId) Select 4,2
		Insert into reference.Nodes (NodeCount, NodeId) Select 4,3
		Insert into reference.Nodes (NodeCount, NodeId) Select 4,4
		Insert into reference.Nodes (NodeCount, NodeId) Select 5,1
		Insert into reference.Nodes (NodeCount, NodeId) Select 5,2
		Insert into reference.Nodes (NodeCount, NodeId) Select 5,3
		Insert into reference.Nodes (NodeCount, NodeId) Select 5,4
		Insert into reference.Nodes (NodeCount, NodeId) Select 5,5
	END;

	IF NOT EXISTS(SELECT 1 FROM reference.Edges)
	BEGIN
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 3,1,2
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 3,2,3
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 3,1,3
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 4,1,2
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 4,1,3
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 4,1,4
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 4,2,3
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 4,2,4
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 4,3,4
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 5,1,2
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 5,1,3
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 5,1,4
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 5,1,5
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 5,2,3
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 5,2,4
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 5,2,5
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 5,3,4
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 5,3,5
		Insert into reference.Edges (NodeCount, NodeIdA, NodeIdB) Select 5,4,5
	END;

	IF NOT EXISTS(SELECT 1 FROM reference.SubGraphSets)
	BEGIN
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,1,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,1,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,1,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,2,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,2,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,2,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,3,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,3,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,3,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,4,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,4,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,4,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,5,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,5,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 3,5,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,1,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,1,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,1,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,1,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,1,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,1,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,2,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,2,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,2,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,2,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,2,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,2,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,3,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,3,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,3,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,3,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,3,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,3,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,4,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,4,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,4,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,4,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,4,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,4,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,5,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,5,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,5,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,5,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,5,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,5,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,6,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,6,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,6,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,6,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,6,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,6,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,7,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,7,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,7,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,7,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,7,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,7,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,8,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,8,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,8,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,8,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,8,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,8,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,9,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,9,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,9,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,9,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,9,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,9,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,10,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,10,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,10,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,10,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,10,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,10,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,11,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,11,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,11,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,11,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,11,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,11,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,12,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,12,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,12,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,12,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,12,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,12,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,13,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,13,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,13,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,13,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,13,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,13,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,14,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,14,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,14,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,14,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,14,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,14,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,15,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,15,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,15,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,15,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,15,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 4,15,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,1,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,1,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,1,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,1,1,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,1,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,1,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,1,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,1,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,1,1,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,1,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,2,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,2,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,2,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,2,1,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,2,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,2,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,2,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,2,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,2,1,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,2,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,3,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,3,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,3,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,3,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,3,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,3,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,3,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,3,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,3,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,3,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,4,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,4,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,4,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,4,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,4,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,4,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,4,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,4,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,4,1,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,4,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,5,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,5,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,5,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,5,1,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,5,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,5,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,5,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,5,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,5,1,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,5,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,6,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,6,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,6,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,6,1,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,6,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,6,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,6,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,6,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,6,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,6,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,7,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,7,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,7,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,7,1,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,7,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,7,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,7,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,7,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,7,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,7,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,8,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,8,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,8,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,8,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,8,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,8,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,8,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,8,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,8,1,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,8,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,9,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,9,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,9,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,9,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,9,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,9,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,9,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,9,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,9,1,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,9,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,10,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,10,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,10,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,10,1,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,10,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,10,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,10,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,10,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,10,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,10,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,11,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,11,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,11,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,11,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,11,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,11,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,11,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,11,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,11,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,11,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,12,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,12,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,12,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,12,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,12,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,12,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,12,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,12,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,12,1,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,12,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,13,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,13,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,13,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,13,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,13,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,13,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,13,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,13,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,13,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,13,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,14,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,14,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,14,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,14,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,14,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,14,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,14,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,14,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,14,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,14,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,15,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,15,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,15,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,15,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,15,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,15,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,15,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,15,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,15,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,15,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,16,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,16,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,16,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,16,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,16,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,16,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,16,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,16,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,16,1,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,16,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,17,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,17,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,17,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,17,1,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,17,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,17,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,17,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,17,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,17,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,17,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,18,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,18,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,18,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,18,1,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,18,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,18,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,18,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,18,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,18,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,18,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,19,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,19,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,19,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,19,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,19,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,19,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,19,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,19,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,19,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,19,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,20,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,20,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,20,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,20,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,20,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,20,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,20,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,20,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,20,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,20,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,21,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,21,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,21,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,21,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,21,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,21,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,21,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,21,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,21,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,21,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,22,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,22,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,22,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,22,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,22,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,22,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,22,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,22,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,22,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,22,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,23,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,23,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,23,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,23,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,23,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,23,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,23,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,23,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,23,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,23,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,24,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,24,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,24,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,24,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,24,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,24,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,24,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,24,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,24,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,24,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,25,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,25,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,25,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,25,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,25,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,25,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,25,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,25,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,25,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,25,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,26,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,26,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,26,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,26,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,26,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,26,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,26,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,26,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,26,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,26,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,27,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,27,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,27,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,27,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,27,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,27,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,27,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,27,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,27,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,27,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,28,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,28,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,28,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,28,1,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,28,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,28,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,28,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,28,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,28,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,28,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,29,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,29,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,29,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,29,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,29,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,29,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,29,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,29,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,29,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,29,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,30,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,30,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,30,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,30,1,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,30,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,30,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,30,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,30,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,30,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,30,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,31,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,31,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,31,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,31,1,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,31,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,31,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,31,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,31,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,31,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,31,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,32,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,32,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,32,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,32,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,32,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,32,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,32,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,32,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,32,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,32,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,33,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,33,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,33,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,33,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,33,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,33,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,33,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,33,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,33,1,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,33,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,34,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,34,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,34,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,34,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,34,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,34,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,34,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,34,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,34,1,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,34,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,35,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,35,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,35,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,35,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,35,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,35,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,35,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,35,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,35,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,35,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,36,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,36,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,36,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,36,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,36,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,36,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,36,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,36,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,36,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,36,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,37,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,37,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,37,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,37,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,37,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,37,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,37,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,37,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,37,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,37,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,38,1,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,38,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,38,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,38,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,38,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,38,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,38,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,38,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,38,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,38,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,39,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,39,1,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,39,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,39,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,39,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,39,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,39,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,39,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,39,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,39,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,40,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,40,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,40,1,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,40,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,40,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,40,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,40,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,40,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,40,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,40,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,41,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,41,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,41,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,41,1,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,41,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,41,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,41,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,41,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,41,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,41,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,42,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,42,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,42,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,42,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,42,1,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,42,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,42,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,42,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,42,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,42,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,43,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,43,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,43,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,43,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,43,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,43,1,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,43,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,43,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,43,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,43,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,44,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,44,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,44,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,44,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,44,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,44,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,44,1,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,44,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,44,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,44,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,45,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,45,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,45,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,45,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,45,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,45,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,45,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,45,1,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,45,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,45,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,46,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,46,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,46,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,46,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,46,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,46,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,46,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,46,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,46,1,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,46,0,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,47,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,47,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,47,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,47,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,47,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,47,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,47,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,47,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,47,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,47,1,3,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,48,0,1,2
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,48,0,2,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,48,0,3,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,48,0,4,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,48,0,1,3
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,48,0,1,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,48,0,2,4
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,48,0,1,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,48,0,2,5
		Insert into reference.SubGraphSets (NodeCount, SubGraphSetId, KeepEdge, NodeIdA, NodeIdB) Select 5,48,0,3,5
	END;

	IF NOT EXISTS(SELECT 1 FROM reference.UPCI2)
	BEGIN
		Drop table if exists #Ten
		Create table #Ten (n tinyint not null, c varchar(1) not null)
		Insert into #Ten Select 0, 'C'
		Insert into #Ten Select 1, 'F'
		Insert into #Ten Select 2, 'G'
		Insert into #Ten Select 3, 'H'
		Insert into #Ten Select 4, 'K'
		Insert into #Ten Select 5, 'L'
		Insert into #Ten Select 6, 'N'
		Insert into #Ten Select 7, 'P'
		Insert into #Ten Select 8, 'R'
		Insert into #Ten Select 9, 'W' 


		Insert into reference.UPCI2
		Select
			cast(c0.n as varchar)+
			c1.c+c2.c+c3.c+
			cast(c4.n as varchar)+cast(c5.n as varchar)+cast(c6.n as varchar)+cast(c7.n as varchar)+
			case
				when right(cast((c0.n*1)+(c1.n*3)+(c2.n*7)+(c3.n*1)+(c4.n*3)+(c5.n*7)+(c6.n*1)+(c7.n*3)%10 as varchar),1) = '0' then 'B'
				when right(cast((c0.n*1)+(c1.n*3)+(c2.n*7)+(c3.n*1)+(c4.n*3)+(c5.n*7)+(c6.n*1)+(c7.n*3)%10 as varchar),1) = '1' then 'C'
				when right(cast((c0.n*1)+(c1.n*3)+(c2.n*7)+(c3.n*1)+(c4.n*3)+(c5.n*7)+(c6.n*1)+(c7.n*3)%10 as varchar),1) = '2' then 'D'
				when right(cast((c0.n*1)+(c1.n*3)+(c2.n*7)+(c3.n*1)+(c4.n*3)+(c5.n*7)+(c6.n*1)+(c7.n*3)%10 as varchar),1) = '3' then 'E'
				when right(cast((c0.n*1)+(c1.n*3)+(c2.n*7)+(c3.n*1)+(c4.n*3)+(c5.n*7)+(c6.n*1)+(c7.n*3)%10 as varchar),1) = '4' then 'F'
				when right(cast((c0.n*1)+(c1.n*3)+(c2.n*7)+(c3.n*1)+(c4.n*3)+(c5.n*7)+(c6.n*1)+(c7.n*3)%10 as varchar),1) = '5' then 'G'
				when right(cast((c0.n*1)+(c1.n*3)+(c2.n*7)+(c3.n*1)+(c4.n*3)+(c5.n*7)+(c6.n*1)+(c7.n*3)%10 as varchar),1) = '6' then 'H'
				when right(cast((c0.n*1)+(c1.n*3)+(c2.n*7)+(c3.n*1)+(c4.n*3)+(c5.n*7)+(c6.n*1)+(c7.n*3)%10 as varchar),1) = '7' then 'J'
				when right(cast((c0.n*1)+(c1.n*3)+(c2.n*7)+(c3.n*1)+(c4.n*3)+(c5.n*7)+(c6.n*1)+(c7.n*3)%10 as varchar),1) = '8' then 'K'
				when right(cast((c0.n*1)+(c1.n*3)+(c2.n*7)+(c3.n*1)+(c4.n*3)+(c5.n*7)+(c6.n*1)+(c7.n*3)%10 as varchar),1) = '9' then 'L'
			end
	
		From 
			#Ten c0,
			#Ten c1,
			#Ten c2,
			#Ten c3,
			#Ten c4,
			#Ten c5,
			#Ten c6,
			#Ten c7
		Where
			c0.n > 0 and
			c1.n <> c2.n and
			c1.n <> c3.n and
			c2.n <> c3.n and
			not(c1.c = 'W' and c2.c = 'N' and c3.c = 'K') and
			not(c1.c = 'W' and c2.c = 'K' and c3.c = 'R') and
			not(c1.c = 'F' and c2.c = 'K' and c3.c = 'R') and
			not(c1.c = 'F' and c2.c = 'C' and c3.c = 'K') and
			c4.n <> c5.n and
			c5.n <> c6.n and
			c6.n <> c7.n
		Order By
			newid()

		Drop table #Ten
	END;

END