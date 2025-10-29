-- =============================================
-- Author:		Paul Cooper
-- Create date: 10/01/2023
-- Description:	Merge Offloc Staging Bookings table with Offloc Running Picture Bookings table
-- =============================================

CREATE PROCEDURE [OfflocRunningPicture].[MergeBookings]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[Bookings] AS target
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [PrisonNumber],
                   [FirstReceptionDate]                   
            FROM [OfflocStagingDb].[OfflocStaging].[Bookings]
        ) AS source
        ([NOMSnumber], [PrisonNumber], [FirstReceptionDate])
        ON (
            target.[NOMSnumber] = source.[NOMSnumber] and
            target.[PrisonNumber] = source.[PrisonNumber]
        )
        WHEN MATCHED AND NOT (
            target.[FirstReceptionDate] = source.[FirstReceptionDate] AND 
            target.[IsActive] = 1
        ) 
        THEN
            UPDATE SET [FirstReceptionDate] = source.[FirstReceptionDate],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [PrisonNumber],
                [FirstReceptionDate],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[PrisonNumber], source.[FirstReceptionDate], 1)
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