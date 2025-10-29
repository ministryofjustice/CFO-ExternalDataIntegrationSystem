-- =============================================
-- Author:		Sam Gibson
-- Create date: 14/06/2024
-- Description:	Runs steps 1, and 2 
-- for use by the matching service
-- =============================================
CREATE PROCEDURE [processing].[Phase1]
AS
BEGIN

    SET NOCOUNT ON;

	EXEC processing.PopulateReferenceTables;

    EXEC processing.Step1_GetPass1Edges;
    EXEC processing.Step2_CompleteEdges;

END