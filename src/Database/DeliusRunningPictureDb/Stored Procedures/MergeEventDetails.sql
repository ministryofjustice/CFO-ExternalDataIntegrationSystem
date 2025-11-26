-- =============================================
-- Author:		Paul Cooper 
-- Create date: 30/01/2023
-- Description:	Merge Delius Staging Event Details table with Delius Running Picture Event Details table
-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeEventDetails]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[EventDetails] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [ReferralDate],
                   [ConvictionDate],
                   [Cohort],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[EventDetails]
        ) AS SOURCE
        ([OffenderId], [Id], [ReferralDate], [ConvictionDate], [Cohort], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND ISNULL(
                                               NULLIF(TARGET.[ReferralDate], SOURCE.[ReferralDate]),
                                               NULLIF(SOURCE.[ReferralDate], TARGET.[ReferralDate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[ConvictionDate], SOURCE.[ConvictionDate]),
                                               NULLIF(SOURCE.[ConvictionDate], TARGET.[ConvictionDate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Cohort], SOURCE.[Cohort]),
                                               NULLIF(SOURCE.[Cohort], TARGET.[Cohort])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OffenderId] = SOURCE.[OffenderId],
                       [ReferralDate] = SOURCE.[ReferralDate],
                       [ConvictionDate] = SOURCE.[ConvictionDate],
                       [Cohort] = SOURCE.[Cohort],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [ReferralDate],
                [ConvictionDate],
                [Cohort],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[ReferralDate], source.[ConvictionDate], source.[Cohort],
             source.[Deleted]);
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