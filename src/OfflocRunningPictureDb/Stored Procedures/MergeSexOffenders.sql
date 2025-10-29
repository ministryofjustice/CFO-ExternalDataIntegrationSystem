CREATE PROCEDURE [OfflocRunningPicture].[MergeSexOffenders]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[SexOffenders] AS TARGET
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [Schedule1SexOffender]
            FROM [OfflocStagingDb].[OfflocStaging].[SexOffenders]
        ) AS SOURCE
        ([NOMSnumber], [Schedule1SexOffender])
        ON (TARGET.[NOMSnumber] = SOURCE.[NOMSnumber])
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.[Schedule1SexOffender], SOURCE.[Schedule1SexOffender]),
                                           NULLIF(SOURCE.[Schedule1SexOffender], TARGET.[Schedule1SexOffender])
                                       ) IS NULL
                                 AND TARGET.[IsActive] = 1
                             ) THEN
            UPDATE SET [Schedule1SexOffender] = SOURCE.[Schedule1SexOffender],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [Schedule1SexOffender],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[Schedule1SexOffender], 1)
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
