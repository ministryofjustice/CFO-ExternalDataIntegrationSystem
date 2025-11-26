-- =============================================
-- Author:		Sam Gibson
-- Create date: 14/06/2024
-- Description:	Runs steps 3, 4, 5, 6, and 7
-- for use by the matching service
-- =============================================
CREATE PROCEDURE [processing].[Phase2]
AS
BEGIN

    SET NOCOUNT ON;

    EXEC processing.Step3_GetPass2Edges;
    EXEC processing.Step4_ResolveGraphs;
    EXEC processing.Step5_MaintainNodeTable;
    EXEC processing.Step6_UpdateClusterTable;
    EXEC processing.Step7_MaintainClusterSearchAttributes;

END;