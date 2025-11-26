-- =============================================
-- Author:		Adam Bennett
-- Create date: 12/06/2024
-- Description:	Runs through all steps for demonstartion
--				Note - procs ending [tbc] need 'wiring up'
--              Placeholder code in place so the whole thing can be tested for now
-- =============================================
CREATE PROCEDURE [processing].[RunThrough]
AS
BEGIN

    SET NOCOUNT ON;

    --Exec processing.FlattenTables --only run this once - when we're live we want the output tables to increment

    EXEC processing.Step1_GetPass1Edges;
    EXEC processing.[Step2_CompleteEdges[tbc]]];
    EXEC processing.Step3_GetPass2Edges;
    EXEC processing.Step4_ResolveGraphs;
    EXEC processing.[Step5_MaintainNodeTable[tbc]]];
    EXEC processing.[Step6_UpdateClusterTable[tbc]]];
    EXEC processing.[Step7_MaintainClusterSearchAttributes[tbc]]];


END;