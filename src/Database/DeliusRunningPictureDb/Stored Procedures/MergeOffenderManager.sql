-- =============================================
-- Author:		Paul Cooper 
-- Create date: 30/01/2023
-- Description:	Merge Delius Staging Offender Manager table with Delius Running Picture Offender Manager table
-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeOffenderManager]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[OffenderManager] AS TARGET
        USING
        (
            SELECT DISTINCT [OmCode],
                   [OmForename],
                   [OmSurname],
                   [Orgcode],
                   [TeamCode],
                   [ContactNo]
            FROM [DeliusStagingDb].[DeliusStaging].[OffenderManager]
        ) AS SOURCE
        ([OmCode], [OmForename], [OmSurname], [Orgcode], [TeamCode], [ContactNo])
        ON (
               TARGET.[OmCode] = SOURCE.[OmCode]
               AND TARGET.[TeamCode] = SOURCE.[TeamCode]
           )
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.[OmForename], SOURCE.[OmForename]),
                                           NULLIF(SOURCE.[OmForename], TARGET.[OmForename])
                                       ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[OmSurname], SOURCE.[OmSurname]),
                                               NULLIF(SOURCE.[OmSurname], TARGET.[OmSurname])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Orgcode], SOURCE.[Orgcode]),
                                               NULLIF(SOURCE.[Orgcode], TARGET.[Orgcode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[ContactNo], SOURCE.[ContactNo]),
                                               NULLIF(SOURCE.[ContactNo], TARGET.[ContactNo])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OmForename] = SOURCE.[OmForename],
                       [OmSurname] = SOURCE.[OmSurname],
                       [Orgcode] = SOURCE.[Orgcode],
                       [ContactNo] = SOURCE.[ContactNo]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OmCode],
                [OmForename],
                [OmSurname],
                [Orgcode],
                [TeamCode],
                [ContactNo]
            )
            VALUES
            (source.[OmCode], source.[OmForename], source.[OmSurname], source.[Orgcode], source.[TeamCode],
             source.[ContactNo]);
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