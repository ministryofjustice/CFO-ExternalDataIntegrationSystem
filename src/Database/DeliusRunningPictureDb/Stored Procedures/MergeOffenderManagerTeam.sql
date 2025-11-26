-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeOffenderManagerTeam]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[OffenderManagerTeam] AS target
        USING
        (
            SELECT DISTINCT [OrgCode],
                   [OrgDescription],
                   [TeamCode],
                   [TeamDescription],
                   [CompositeBuildingHash]
            FROM [DeliusStagingDb].[DeliusStaging].[OffenderManagerTeam]
        ) AS source
        ([OrgCode], [OrgDescription], [TeamCode], [TeamDescription], [CompositeBuildingHash])
        ON (
               target.[OrgCode] = source.[OrgCode]
               AND target.[TeamCode] = source.[TeamCode]
               AND target.[CompositeBuildingHash] = source.[CompositeBuildingHash]
           )
        WHEN MATCHED AND NOT (

                                       ISNULL(
                                           NULLIF(TARGET.[OrgDescription], SOURCE.[OrgDescription]),
                                           NULLIF(SOURCE.[OrgDescription], TARGET.[OrgDescription])
                                       ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[TeamDescription], SOURCE.[TeamDescription]),
                                               NULLIF(SOURCE.[TeamDescription], TARGET.[TeamDescription])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OrgDescription] = source.[OrgDescription],
                       [TeamDescription] = source.[TeamDescription]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OrgCode],
                [OrgDescription],
                [TeamCode],
                [TeamDescription],
                [CompositeBuildingHash]
            )
            VALUES
            (source.[OrgCode], source.[OrgDescription], source.[TeamCode], source.[TeamDescription],
             source.[CompositeBuildingHash]);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;
        ; THROW;
    END CATCH;
END;
