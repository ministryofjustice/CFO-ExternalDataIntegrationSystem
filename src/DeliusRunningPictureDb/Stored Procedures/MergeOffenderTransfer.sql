-- =============================================
-- Author:		Paul Cooper 
-- Create date: 30/01/2023
-- Description:	Merge Delius Staging Offender Transfer table with Delius Running Picture Offender Transfer table
-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeOffenderTransfer]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[OffenderTransfer] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [RequestDate],
                   [ReasonCode],
                   [ReasonDescription],
                   [StatusCode],
                   [StatusDescription],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[OffenderTransfer]
        ) AS SOURCE
        ([OffenderId], [Id], [RequestDate], [ReasonCode], [ReasonDescription], [StatusCode], [StatusDescription], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND ISNULL(
                                               NULLIF(TARGET.[RequestDate], SOURCE.[RequestDate]),
                                               NULLIF(SOURCE.[RequestDate], TARGET.[RequestDate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[ReasonCode], SOURCE.[ReasonCode]),
                                               NULLIF(SOURCE.[ReasonCode], TARGET.[ReasonCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[ReasonDescription], SOURCE.[ReasonDescription]),
                                               NULLIF(SOURCE.[ReasonDescription], TARGET.[ReasonDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[StatusCode], SOURCE.[StatusCode]),
                                               NULLIF(SOURCE.[StatusCode], TARGET.[StatusCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[StatusDescription], SOURCE.[StatusDescription]),
                                               NULLIF(SOURCE.[StatusDescription], TARGET.[StatusDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OffenderId] = SOURCE.[OffenderId],
                       [RequestDate] = SOURCE.[RequestDate],
                       [ReasonCode] = SOURCE.[ReasonCode],
                       [ReasonDescription] = SOURCE.[ReasonDescription],
                       [StatusCode] = SOURCE.[StatusCode],
                       [StatusDescription] = SOURCE.[StatusDescription],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [RequestDate],
                [ReasonCode],
                [ReasonDescription],
                [StatusCode],
                [StatusDescription],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[RequestDate], source.[ReasonCode], source.[ReasonDescription],
             source.[StatusCode], source.[StatusDescription], source.[Deleted]);
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