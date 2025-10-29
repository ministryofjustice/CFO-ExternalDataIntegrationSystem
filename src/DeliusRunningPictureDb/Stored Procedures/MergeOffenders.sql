-- =============================================
-- Author:		Paul Cooper & Dan Fenelon
-- Create date: 29/01/2023
-- Description:	Merge Delius Staging Offenders table with Delius Running Picture Offenders table
-- =============================================
-- Author:		Paul Cooper
-- Create date: 25/04/2023
-- Description:	Add is null/ null if Checks
-- =============================================

CREATE PROCEDURE [DeliusRunningPicture].[MergeOffenders]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[Offenders] AS TARGET
        USING
        (
            SELECT DISTINCT [OffenderId],
                   [Id],
                   [FirstName],
                   [SecondName],
                   [ThirdName],
                   [Surname],
                   [PreviousSurname],
                   [TitleCode],
                   [TitleDescription],
                   [CRN],
                   [CRO],
                   [NOMISNumber],
                   [PNCNumber],
                   [NINO],
                   [ImmigrationNumber],
                   [GenderCode],
                   [GenderDescription],
                   [DateOfBirth],
                   [NationalityCode],
                   [NationalityDescription],
                   [SecondNationalityCode],
                   [SecondNationalityDescription],
                   [ImmigrationStatusCode],
                   [ImmigrationStatusDescription],
                   [EthnicityCode],
                   [EthnicityDescription],
                   [PrisonNumber],
                   [Deleted]
            FROM [DeliusStagingDb].[DeliusStaging].[Offenders]
        ) AS SOURCE
        ([OffenderId], [Id], [FirstName], [SecondName], [ThirdName], [Surname], [PreviousSurname], [TitleCode],
        [TitleDescription], [CRN], [CRO], [NOMISNumber], [PNCNumber], [NINO], [ImmigrationNumber], [GenderCode], 
        [GenderDescription], [DateOfBirth], [NationalityCode], [NationalityDescription], [SecondNationalityCode], 
        [SecondNationalityDescription], [ImmigrationStatusCode], [ImmigrationStatusDescription], [EthnicityCode], [EthnicityDescription], [PrisonNumber], [Deleted])
        ON (TARGET.[Id] = SOURCE.[Id])
        WHEN MATCHED AND NOT (
                                 TARGET.[OffenderId] = SOURCE.[OffenderId]
                                 AND ISNULL(
                                               NULLIF(TARGET.[FirstName], SOURCE.[FirstName]),
                                               NULLIF(SOURCE.[FirstName], TARGET.[FirstName])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[SecondName], SOURCE.[SecondName]),
                                               NULLIF(SOURCE.[SecondName], TARGET.[SecondName])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[ThirdName], SOURCE.[ThirdName]),
                                               NULLIF(SOURCE.[ThirdName], TARGET.[ThirdName])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Surname], SOURCE.[Surname]),
                                               NULLIF(SOURCE.[Surname], TARGET.[Surname])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[PreviousSurname], SOURCE.[PreviousSurname]),
                                               NULLIF(SOURCE.[PreviousSurname], TARGET.[PreviousSurname])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[TitleCode], SOURCE.[TitleCode]),
                                               NULLIF(SOURCE.[TitleCode], TARGET.[TitleCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[TitleDescription], SOURCE.[TitleDescription]),
                                               NULLIF(SOURCE.[TitleDescription], TARGET.[TitleDescription])
                                           ) IS NULL
                                 AND ISNULL(NULLIF(TARGET.[CRN], SOURCE.[CRN]), NULLIF(SOURCE.[CRN], TARGET.[CRN])) IS NULL
                                 AND ISNULL(NULLIF(TARGET.[CRO], SOURCE.[CRO]), NULLIF(SOURCE.[CRO], TARGET.[CRO])) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[NOMISNumber], SOURCE.[NOMISNumber]),
                                               NULLIF(SOURCE.[NOMISNumber], TARGET.[NOMISNumber])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[PNCNumber], SOURCE.[PNCNumber]),
                                               NULLIF(SOURCE.[PNCNumber], TARGET.[PNCNumber])
                                           ) IS NULL
                                 AND ISNULL(NULLIF(TARGET.[NINO], SOURCE.[NINO]), NULLIF(SOURCE.[NINO], TARGET.[NINO])) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[ImmigrationNumber], SOURCE.[ImmigrationNumber]),
                                               NULLIF(SOURCE.[ImmigrationNumber], TARGET.[ImmigrationNumber])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[GenderCode], SOURCE.[GenderCode]),
                                               NULLIF(SOURCE.[GenderCode], TARGET.[GenderCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[GenderDescription], SOURCE.[GenderDescription]),
                                               NULLIF(SOURCE.[GenderDescription], TARGET.[GenderDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[DateOfBirth], SOURCE.[DateOfBirth]),
                                               NULLIF(SOURCE.[DateOfBirth], TARGET.[DateOfBirth])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[NationalityCode], SOURCE.[NationalityCode]),
                                               NULLIF(SOURCE.[NationalityCode], TARGET.[NationalityCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[NationalityDescription], SOURCE.[NationalityDescription]),
                                               NULLIF(SOURCE.[NationalityDescription], TARGET.[NationalityDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[SecondNationalityCode], SOURCE.[SecondNationalityCode]),
                                               NULLIF(SOURCE.[SecondNationalityCode], TARGET.[SecondNationalityCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(
                                                         TARGET.[SecondNationalityDescription],
                                                         SOURCE.[SecondNationalityDescription]
                                                     ),
                                               NULLIF(
                                                         SOURCE.[SecondNationalityDescription],
                                                         TARGET.[SecondNationalityDescription]
                                                     )
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[ImmigrationStatusCode], SOURCE.[ImmigrationStatusCode]),
                                               NULLIF(SOURCE.[ImmigrationStatusCode], TARGET.[ImmigrationStatusCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(
                                                         TARGET.[ImmigrationStatusDescription],
                                                         SOURCE.[ImmigrationStatusDescription]
                                                     ),
                                               NULLIF(
                                                         SOURCE.[ImmigrationStatusDescription],
                                                         TARGET.[ImmigrationStatusDescription]
                                                     )
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[EthnicityCode], SOURCE.[EthnicityCode]),
                                               NULLIF(SOURCE.[EthnicityCode], TARGET.[EthnicityCode])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[EthnicityDescription], SOURCE.[EthnicityDescription]),
                                               NULLIF(SOURCE.[EthnicityDescription], TARGET.[EthnicityDescription])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[PrisonNumber], SOURCE.[PrisonNumber]),
                                               NULLIF(SOURCE.[PrisonNumber], TARGET.[PrisonNumber])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[Deleted], SOURCE.[Deleted]),
                                               NULLIF(SOURCE.[Deleted], TARGET.[Deleted])
                                           ) IS NULL
                             ) THEN
            UPDATE SET [OffenderId] = SOURCE.[OffenderId],
                       [FirstName] = SOURCE.[FirstName],
                       [SecondName] = SOURCE.[SecondName],
                       [ThirdName] = SOURCE.[ThirdName],
                       [Surname] = SOURCE.[Surname],
                       [PreviousSurname] = SOURCE.[PreviousSurname],
                       [TitleCode] = SOURCE.[TitleCode],
                       [TitleDescription] = SOURCE.[TitleDescription],
                       [CRN] = SOURCE.[CRN],
                       [CRO] = SOURCE.[CRO],
                       [NOMISNumber] = SOURCE.[NOMISNumber],
                       [PNCNumber] = SOURCE.[PNCNumber],
                       [NINO] = SOURCE.[NINO],
                       [ImmigrationNumber] = SOURCE.[ImmigrationNumber],
                       [GenderCode] = SOURCE.[GenderCode],
                       [GenderDescription] = SOURCE.[GenderDescription],
                       [DateOfBirth] = SOURCE.[DateOfBirth],
                       [NationalityCode] = SOURCE.[NationalityCode],
                       [NationalityDescription] = SOURCE.[NationalityDescription],
                       [SecondNationalityCode] = SOURCE.[SecondNationalityCode],
                       [SecondNationalityDescription] = SOURCE.[SecondNationalityDescription],
                       [ImmigrationStatusCode] = SOURCE.[ImmigrationStatusCode],
                       [ImmigrationStatusDescription] = SOURCE.[ImmigrationStatusDescription],
                       [EthnicityCode] = SOURCE.[EthnicityCode],
                       [EthnicityDescription] = SOURCE.[EthnicityDescription],
                       [PrisonNumber] = SOURCE.[PrisonNumber],
                       [Deleted] = SOURCE.[Deleted]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [OffenderId],
                [Id],
                [FirstName],
                [SecondName],
                [ThirdName],
                [Surname],
                [PreviousSurname],
                [TitleCode],
                [TitleDescription],
                [CRN],
                [CRO],
                [NOMISNumber],
                [PNCNumber],
                [NINO],
                [ImmigrationNumber],
                [GenderCode],
                [GenderDescription],
                [DateOfBirth],
                [NationalityCode],
                [NationalityDescription],
                [SecondNationalityCode],
                [SecondNationalityDescription],
                [ImmigrationStatusCode],
                [ImmigrationStatusDescription],
                [EthnicityCode],
                [EthnicityDescription],
                [PrisonNumber],
                [Deleted]
            )
            VALUES
            (source.[OffenderId], source.[Id], source.[FirstName], source.[SecondName], source.[ThirdName],
             source.[Surname], source.[PreviousSurname], source.[TitleCode], source.[TitleDescription], source.[CRN],
             source.[CRO], source.[NOMISNumber], source.[PNCNumber], source.[NINO], source.[ImmigrationNumber],
             source.[GenderCode], source.[GenderDescription], source.[DateOfBirth], source.[NationalityCode],
             source.[NationalityDescription], source.[SecondNationalityCode], source.[SecondNationalityDescription],
             source.[ImmigrationStatusCode], source.[ImmigrationStatusDescription], source.[EthnicityCode],
             source.[EthnicityDescription], source.[PrisonNumber], source.[Deleted]);
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