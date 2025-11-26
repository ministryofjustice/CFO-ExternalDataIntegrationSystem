CREATE PROCEDURE [OfflocRunningPicture].[MergeSentenceInformation]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [OfflocRunningPicture].[SentenceInformation] AS TARGET
        USING
        (
            SELECT DISTINCT [NOMSnumber],
                   [FirstSentenced],
                   [SentenceYears],
                   [SentenceMonths],
                   [SentenceDays],
                   [EarliestPossibleReleaseDate],
                   [DateOfRelease],
                   [sed],
                   [hdced],
                   [hdcad],
                   [ped],
                   [crd],
                   [npd],
                   [led],
                   [tused]
            FROM [OfflocStagingDb].[OfflocStaging].[SentenceInformation]
        ) AS SOURCE
        ([NOMSnumber], [FirstSentenced], [SentenceYears], [SentenceMonths], [SentenceDays], [EarliestPossibleReleaseDate], [DateOfRelease], [sed], [hdced], [hdcad], [ped], [crd], [npd], [led], [tused])
        ON (TARGET.[NOMSnumber] = SOURCE.[NOMSnumber])
        WHEN MATCHED AND NOT (
                                 ISNULL(
                                           NULLIF(TARGET.[FirstSentenced], SOURCE.[FirstSentenced]),
                                           NULLIF(SOURCE.[FirstSentenced], TARGET.[FirstSentenced])
                                       ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[SentenceYears], SOURCE.[SentenceYears]),
                                               NULLIF(SOURCE.[SentenceYears], TARGET.[SentenceYears])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[SentenceMonths], SOURCE.[SentenceMonths]),
                                               NULLIF(SOURCE.[SentenceMonths], TARGET.[SentenceMonths])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[SentenceDays], SOURCE.[SentenceDays]),
                                               NULLIF(SOURCE.[SentenceDays], TARGET.[SentenceDays])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(
                                                         TARGET.[EarliestPossibleReleaseDate],
                                                         SOURCE.[EarliestPossibleReleaseDate]
                                                     ),
                                               NULLIF(
                                                         SOURCE.[EarliestPossibleReleaseDate],
                                                         TARGET.[EarliestPossibleReleaseDate]
                                                     )
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[DateOfRelease], SOURCE.[DateOfRelease]),
                                               NULLIF(SOURCE.[DateOfRelease], TARGET.[DateOfRelease])
                                           ) IS NULL
                                 AND ISNULL(NULLIF(TARGET.[sed], SOURCE.[sed]), NULLIF(SOURCE.[sed], TARGET.[sed])) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[hdced], SOURCE.[hdced]),
                                               NULLIF(SOURCE.[hdced], TARGET.[hdced])
                                           ) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[hdcad], SOURCE.[hdcad]),
                                               NULLIF(SOURCE.[hdcad], TARGET.[hdcad])
                                           ) IS NULL
                                 AND ISNULL(NULLIF(TARGET.[ped], SOURCE.[ped]), NULLIF(SOURCE.[ped], TARGET.[ped])) IS NULL
                                 AND ISNULL(NULLIF(TARGET.[crd], SOURCE.[crd]), NULLIF(SOURCE.[crd], TARGET.[crd])) IS NULL
                                 AND ISNULL(NULLIF(TARGET.[npd], SOURCE.[npd]), NULLIF(SOURCE.[npd], TARGET.[npd])) IS NULL
                                 AND ISNULL(NULLIF(TARGET.[led], SOURCE.[led]), NULLIF(SOURCE.[led], TARGET.[led])) IS NULL
                                 AND ISNULL(
                                               NULLIF(TARGET.[tused], SOURCE.[tused]),
                                               NULLIF(SOURCE.[tused], TARGET.[tused])
                                           ) IS NULL
                                 AND TARGET.[IsActive] = 1
                             ) THEN
            UPDATE SET [FirstSentenced] = SOURCE.[FirstSentenced],
                       [SentenceYears] = SOURCE.[SentenceYears],
                       [SentenceMonths] = SOURCE.[SentenceMonths],
                       [SentenceDays] = SOURCE.[SentenceDays],
                       [EarliestPossibleReleaseDate] = SOURCE.[EarliestPossibleReleaseDate],
                       [DateOfRelease] = SOURCE.[DateOfRelease],
                       [sed] = SOURCE.[sed],
                       [hdced] = SOURCE.[hdced],
                       [hdcad] = SOURCE.[hdcad],
                       [ped] = SOURCE.[ped],
                       [crd] = SOURCE.[crd],
                       [npd] = SOURCE.[npd],
                       [led] = SOURCE.[led],
                       [tused] = SOURCE.[tused],
                       [IsActive] = 1
        WHEN NOT MATCHED THEN
            INSERT
            (
                [NOMSnumber],
                [FirstSentenced],
                [SentenceYears],
                [SentenceMonths],
                [SentenceDays],
                [EarliestPossibleReleaseDate],
                [DateOfRelease],
                [sed],
                [hdced],
                [hdcad],
                [ped],
                [crd],
                [npd],
                [led],
                [tused],
                [IsActive]
            )
            VALUES
            (source.[NOMSnumber], source.[FirstSentenced], source.[SentenceYears], source.[SentenceMonths],
             source.[SentenceDays], source.[EarliestPossibleReleaseDate], source.[DateOfRelease], source.[sed],
             source.[hdced], source.[hdcad], source.[ped], source.[crd], source.[npd], source.[led], source.[tused], 1)
        WHEN NOT MATCHED BY SOURCE AND IsActive = 1 THEN
            UPDATE SET IsActive = 0;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;
        THROW;
    END CATCH;
END;
