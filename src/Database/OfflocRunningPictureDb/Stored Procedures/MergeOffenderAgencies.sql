-- =============================================
-- Author:		Paul Cooper
-- Create date: 17/01/2023
-- Description:	Merge Offloc Staging Offender Agencies table with Offloc Running Picture Offender Agencies table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeOffenderAgencies]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[OffenderAgencies] AS target
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [EstablishmentCode]                 
            FROM [OfflocStagingDb].[OfflocStaging].[OffenderAgencies]
        ) AS source
        ([NOMSnumber], [EstablishmentCode])
        ON (target.[NOMSnumber] = source.[NOMSnumber])
        WHEN MATCHED AND NOT(
            target.[EstablishmentCode] = source.[EstablishmentCode] AND 
            target.[IsActive] = 1
        ) 
        THEN UPDATE SET 
            target.[EstablishmentCode] = source.[EstablishmentCode],
            target.[IsActive] = 1

        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [EstablishmentCode],
                [IsActive]
            )
            VALUES (source.[NOMSnumber], source.[EstablishmentCode], 1)
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