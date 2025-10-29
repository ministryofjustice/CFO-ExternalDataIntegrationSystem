-- =============================================
-- Author:		Paul Cooper
-- Create date: 17/01/2023
-- Description:	Merge Offloc Staging Identifiers table with Offloc Running Picture Identifiers table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeIdentifiers]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[Identifiers] AS target
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [CROno]
            FROM [OfflocStagingDb].[OfflocStaging].[Identifiers]
        ) AS source
        ([NOMSnumber], [CROno])
        ON (target.[NOMSnumber] = source.[NOMSnumber])
        WHEN MATCHED AND NOT (
            target.[CROno] = source.[CROno] AND 
            target.[IsActive] = 1
        ) THEN
            UPDATE SET [CROno] = source.[CROno],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [CROno],
                [IsActive] 
            )
            VALUES
            (source.[NOMSnumber], source.[CROno], 1)
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