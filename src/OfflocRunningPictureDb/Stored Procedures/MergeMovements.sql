-- =============================================
-- Author:		Paul Cooper
-- Create date: 17/01/2023
-- Description:	Merge Offloc Staging Movements table with Offloc Running Picture Movements table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeMovements]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[Movements] AS TARGET
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [MovementEstabComponent],
                   [MovementCode],
                   [TransferReason],
                   [DateOfMovement],
                   [HourOfMovement],
                   [MinOfMovement],
                   [SecOfMovement]
            FROM [OfflocStagingDb].[OfflocStaging].[Movements]
        ) AS SOURCE
        ([NOMSnumber], [MovementEstabComponent], [MovementCode], [TransferReason], [DateOfMovement], [HourOfMovement], [MinOfMovement], [SecOfMovement])
        ON (TARGET.[NOMSnumber] = SOURCE.[NOMSnumber])
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.[MovementEstabComponent], SOURCE.[MovementEstabComponent]),
                                           NULLIF(SOURCE.[MovementEstabComponent], TARGET.[MovementEstabComponent])
                                       ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[MovementCode], SOURCE.[MovementCode]),
                                               NULLIF(SOURCE.[MovementCode], TARGET.[MovementCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[TransferReason], SOURCE.[TransferReason]),
                                               NULLIF(SOURCE.[TransferReason], TARGET.[TransferReason])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[DateOfMovement], SOURCE.[DateOfMovement]),
                                               NULLIF(SOURCE.[DateOfMovement], TARGET.[DateOfMovement])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[HourOfMovement], SOURCE.[HourOfMovement]),
                                               NULLIF(SOURCE.[HourOfMovement], TARGET.[HourOfMovement])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[MinOfMovement], SOURCE.[MinOfMovement]),
                                               NULLIF(SOURCE.[MinOfMovement], TARGET.[MinOfMovement])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[SecOfMovement], SOURCE.[SecOfMovement]),
                                               NULLIF(SOURCE.[SecOfMovement], TARGET.[SecOfMovement])
                                           ) IS NULL
                                 AND TARGET.[IsActive] = 1
                             ) THEN
            UPDATE SET [MovementEstabComponent] = SOURCE.[MovementEstabComponent],
                       [MovementCode] = SOURCE.[MovementCode],
                       [TransferReason] = SOURCE.[TransferReason],
                       [DateOfMovement] = SOURCE.[DateOfMovement],
                       [HourOfMovement] = SOURCE.[HourOfMovement],
                       [MinOfMovement] = SOURCE.[MinOfMovement],
                       [SecOfMovement] = SOURCE.[SecOfMovement],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [MovementEstabComponent],
                [MovementCode],
                [TransferReason],
                [DateOfMovement],
                [HourOfMovement],
                [MinOfMovement],
                [SecOfMovement],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[MovementEstabComponent], source.[MovementCode], source.[TransferReason],
             source.[DateOfMovement], source.[HourOfMovement], source.[MinOfMovement], source.[SecOfMovement], 1)
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