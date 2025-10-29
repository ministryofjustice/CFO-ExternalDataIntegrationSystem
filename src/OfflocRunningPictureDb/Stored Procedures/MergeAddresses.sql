-- =============================================
-- Author:		Paul Cooper
-- Create date: 10/01/2023
-- Description:	Merge Offloc Staging Addresses table with Offloc Running Picture addresses table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeAddresses]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY

        MERGE [OfflocRunningPicture].[Addresses] AS TARGET
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [AddressType],
                   [NominatedNOK],
                   [AddressRelationship],
                   [Address1],
                   [Address2],
                   [Address3],
                   [Address4],
                   [Address5],
                   [Address6],
                   [Address7]
            FROM [OfflocStagingDb].[OfflocStaging].[Addresses]
        ) AS SOURCE
        ([NOMSnumber], [AddressType], [NominatedNOK], [AddressRelationship], [Address1], [Address2], [Address3], [Address4], [Address5], [Address6], [Address7])
        ON (
               TARGET.[NOMSnumber] = SOURCE.[NOMSnumber]
               AND TARGET.[AddressType] = SOURCE.[AddressType]
           )
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.NominatedNOK, SOURCE.NominatedNOK),
                                           NULLIF(SOURCE.NominatedNOK, TARGET.NominatedNOK)
                                       ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.AddressRelationship, SOURCE.AddressRelationship),
                                               NULLIF(SOURCE.AddressRelationship, TARGET.AddressRelationship)
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.Address1, SOURCE.Address1),
                                               NULLIF(SOURCE.Address1, TARGET.Address1)
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.Address2, SOURCE.Address2),
                                               NULLIF(SOURCE.Address2, TARGET.Address2)
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.Address3, SOURCE.Address3),
                                               NULLIF(SOURCE.Address3, TARGET.Address3)
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.Address4, SOURCE.Address4),
                                               NULLIF(SOURCE.Address4, TARGET.Address4)
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.Address5, SOURCE.Address5),
                                               NULLIF(SOURCE.Address5, TARGET.Address5)
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.Address6, SOURCE.Address6),
                                               NULLIF(SOURCE.Address6, TARGET.Address6)
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.Address7, SOURCE.Address7),
                                               NULLIF(SOURCE.Address7, TARGET.Address7)
                                           ) IS NULL
                                 AND TARGET.IsActive = 1
                             ) THEN
            UPDATE SET [NominatedNOK] = SOURCE.[NominatedNOK],
                       [AddressRelationship] = SOURCE.[AddressRelationship],
                       [Address1] = SOURCE.[Address1],
                       [Address2] = SOURCE.[Address2],
                       [Address3] = SOURCE.[Address3],
                       [Address4] = SOURCE.[Address4],
                       [Address5] = SOURCE.[Address5],
                       [Address6] = SOURCE.[Address6],
                       [Address7] = SOURCE.[Address7],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [AddressType],
                [NominatedNOK],
                [AddressRelationship],
                [Address1],
                [Address2],
                [Address3],
                [Address4],
                [Address5],
                [Address6],
                [Address7],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[AddressType], source.[NominatedNOK], source.[AddressRelationship],
             source.[Address1], source.[Address2], source.[Address3], source.[Address4], source.[Address5],
             source.[Address6], source.[Address7], 1)
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