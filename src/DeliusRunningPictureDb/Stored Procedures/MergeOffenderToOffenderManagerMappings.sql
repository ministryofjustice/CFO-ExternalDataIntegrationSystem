-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeOffenderToOffenderManagerMappings]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[OffenderToOffenderManagerMappings] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [AllocatedDate],
                   [EndDate],
                   [OmCode],
                   [OrgCode],
                   [TeamCode],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[OffenderToOffenderManagerMappings]
        ) AS SOURCE
        ([OffenderId], [Id], [AllocatedDate], [EndDate], [OmCode], [OrgCode], [TeamCode], [Deleted])
        ON (
               TARGET.[OffenderId] = SOURCE.[OffenderId]
               AND TARGET.[Id] = SOURCE.[Id]
               AND TARGET.[OmCode] = SOURCE.[OmCode]
           )
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.[AllocatedDate], SOURCE.[AllocatedDate]),
                                           NULLIF(SOURCE.[AllocatedDate], TARGET.[AllocatedDate])
                                       ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[EndDate], SOURCE.[EndDate]),
                                               NULLIF(SOURCE.[EndDate], TARGET.[EndDate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[OrgCode], SOURCE.[OrgCode]),
                                               NULLIF(SOURCE.[OrgCode], TARGET.[OrgCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[TeamCode], SOURCE.[TeamCode]),
                                               NULLIF(SOURCE.[TeamCode], TARGET.[TeamCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [AllocatedDate] = SOURCE.[AllocatedDate],
                       [EndDate] = SOURCE.[EndDate],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [AllocatedDate],
                [EndDate],
                [OmCode],
                [OrgCode],
                [TeamCode],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[AllocatedDate], source.[EndDate], source.[OmCode], source.[OrgCode], source.[TeamCode],
             source.[Deleted]);
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