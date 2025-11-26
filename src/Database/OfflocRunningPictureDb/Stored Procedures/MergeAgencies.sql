-- =============================================
-- Author:		Paul Cooper
-- Create date: 10/01/2023
-- Description:	Merge Offloc Staging Agencies table with Offloc Running Picture Agencies table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeAgencies]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY

        MERGE [OfflocRunningPicture].[Agencies] AS target
        USING
        (
            SELECT DISTINCT [EstablishmentCode],
                   [Establishment]
            FROM [OfflocStagingDb].[OfflocStaging].[Agencies]
        ) AS source
        ([EstablishmentCode], [Establishment])
        ON (
            target.[EstablishmentCode] = source.[EstablishmentCode]
        )
        WHEN MATCHED AND NOT (target.[Establishment] = source.[Establishment]) THEN 
            UPDATE SET [Establishment] = source.[Establishment]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT
            (
                [EstablishmentCode],
                [Establishment]
            )
            VALUES(source.[EstablishmentCode], source.[Establishment])
        WHEN NOT MATCHED BY SOURCE THEN DELETE;
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