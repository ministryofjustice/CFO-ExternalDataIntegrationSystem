-- =============================================
-- Author:		Paul Cooper 
-- Create date: 30/01/2023
-- Description:	Merge Delius Staging Offender Address table with Delius Running Picture Offender Address table
-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeOffenderAddress]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[OffenderAddress] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [StatusCode],
                   [StatusDescription],
                   [BuildingName],
                   [HouseNumber],
                   [StreetName],
                   [District],
                   [Town],
                   [County],
                   [Postcode],
                   [StartDate],
                   [NoFixedAbode],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[OffenderAddress]
        ) AS SOURCE
        ([OffenderId], [Id], [StatusCode], [StatusDescription], [BuildingName], [HouseNumber], [StreetName], [District], [Town], [County], [Postcode], [StartDate], [NoFixedAbode], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND ISNULL(
                                               NULLIF(TARGET.[StatusCode], SOURCE.[StatusCode]),
                                               NULLIF(SOURCE.[StatusCode], TARGET.[StatusCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[StatusDescription], SOURCE.[StatusDescription]),
                                               NULLIF(SOURCE.[StatusDescription], TARGET.[StatusDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[BuildingName], SOURCE.[BuildingName]),
                                               NULLIF(SOURCE.[BuildingName], TARGET.[BuildingName])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[HouseNumber], SOURCE.[HouseNumber]),
                                               NULLIF(SOURCE.[HouseNumber], TARGET.[HouseNumber])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[StreetName], SOURCE.[StreetName]),
                                               NULLIF(SOURCE.[StreetName], TARGET.[StreetName])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[District], SOURCE.[District]),
                                               NULLIF(SOURCE.[District], TARGET.[District])
                                           ) IS NULL
                                 AND ISNULL(NULLIF(TARGET.[Town], SOURCE.[Town]), NULLIF(SOURCE.[Town], TARGET.[Town])) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[County], SOURCE.[County]),
                                               NULLIF(SOURCE.[County], TARGET.[County])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Postcode], SOURCE.[Postcode]),
                                               NULLIF(SOURCE.[Postcode], TARGET.[Postcode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[StartDate], SOURCE.[StartDate]),
                                               NULLIF(SOURCE.[StartDate], TARGET.[StartDate])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[NoFixedAbode], SOURCE.[NoFixedAbode]),
                                               NULLIF(SOURCE.[NoFixedAbode], TARGET.[NoFixedAbode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OffenderId] = SOURCE.[OffenderId],
                       [StatusCode] = SOURCE.[StatusCode],
                       [StatusDescription] = SOURCE.[StatusDescription],
                       [BuildingName] = SOURCE.[BuildingName],
                       [HouseNumber] = SOURCE.[HouseNumber],
                       [StreetName] = SOURCE.[StreetName],
                       [District] = SOURCE.[District],
                       [Town] = SOURCE.[Town],
                       [County] = SOURCE.[County],
                       [Postcode] = SOURCE.[Postcode],
                       [StartDate] = SOURCE.[StartDate],
                       [NoFixedAbode] = SOURCE.[NoFixedAbode],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [StatusCode],
                [StatusDescription],
                [BuildingName],
                [HouseNumber],
                [StreetName],
                [District],
                [Town],
                [County],
                [Postcode],
                [StartDate],
                [NoFixedAbode],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[StatusCode], source.[StatusDescription], source.[BuildingName],
             source.[HouseNumber], source.[StreetName], source.[District], source.[Town], source.[County],
             source.[Postcode], source.[StartDate], source.[NoFixedAbode], source.[Deleted]);
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