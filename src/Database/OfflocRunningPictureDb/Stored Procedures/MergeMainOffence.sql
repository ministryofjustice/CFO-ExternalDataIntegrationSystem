-- =============================================
-- Author:		Paul Cooper
-- Create date: 17/01/2023
-- Description:	Merge Offloc Staging Offence table with Offloc Running Picture Offence table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeMainOffence]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[MainOffence] AS TARGET
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [MainOffence],
                   [DateFirstConviction]
            FROM [OfflocStagingDb].[OfflocStaging].[MainOffence]
        ) AS SOURCE
        ([NOMSnumber], [MainOffence], [DateFirstConviction])
        ON (TARGET.[NOMSnumber] = SOURCE.[NOMSnumber])
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.[MainOffence], SOURCE.[MainOffence]),
                                           NULLIF(SOURCE.[MainOffence], TARGET.[MainOffence])
                                       ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[DateFirstConviction], SOURCE.[DateFirstConviction]),
                                               NULLIF(SOURCE.[DateFirstConviction], TARGET.[DateFirstConviction])
                                           ) IS NULL
                                 AND TARGET.[IsActive] = 1
                             ) THEN
            UPDATE SET [MainOffence] = SOURCE.[MainOffence],
                       [DateFirstConviction] = SOURCE.[DateFirstConviction],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [MainOffence],
                [DateFirstConviction],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[MainOffence], source.[DateFirstConviction], 1)
        WHEN NOT MATCHED BY SOURCE AND IsActive = 1 THEN
            UPDATE SET IsActive = 0;
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