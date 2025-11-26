CREATE PROCEDURE [DeliusRunningPicture].[MergeOffenderManagerBuildings]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[OffenderManagerBuildings] AS TARGET
        USING
        (
            SELECT DISTINCT [CompositeHash],
                   [BuildingName],
                   [PostCode],
                   [HouseNumber],
                   [Street],
                   [District],
                   [Town],
                   [County]
            FROM [DeliusStagingDb].[DeliusStaging].[OffenderManagerBuildings]
        ) AS SOURCE
        ([CompositeHash], [BuildingName], [PostCode], [HouseNumber], [Street], [District], [Town], [County])
        ON (TARGET.[CompositeHash] = SOURCE.[CompositeHash])
        WHEN NOT MATCHED THEN
            INSERT
            (
                [CompositeHash],
                [BuildingName],
                [PostCode],
                [HouseNumber],
                [Street],
                [District],
                [Town],
                [County]
            )
            VALUES
            (source.[CompositeHash], source.[BuildingName], source.[PostCode], source.[HouseNumber], source.[Street],
             source.[District], source.[Town], source.[County]);
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
