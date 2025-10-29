CREATE PROCEDURE [DeliusRunningPicture].[MergeStandardisationReference]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        MERGE [DeliusRunningPicture].[StandardisationReference] AS target
        USING
        (
            SELECT [RawData],
                   [CleanedData],
                   [Key],
                   [KeyColumnName],
                   [Source],
                   [Type]
            FROM [DeliusStagingDb].[DeliusStaging].[StandardisationReference]
        ) AS source
		([RawData], [CleanedData], [Key], [KeyColumnName], [Source], [Type])
        ON (target.[RawData]=source.[RawData] AND target.[CleanedData] = source.[CleanedData] 
		AND target.[Key] = source.[Key] AND target.[KeyColumnName] = source.[KeyColumnName]
		AND target.[Source]=source.[Source] AND target.[Type] = source.[Type])
        WHEN MATCHED AND NOT (
                                target.[CleanedData] = source.[CleanedData]
                                AND target.[Key] = source.[Key]
                                AND target.[KeyColumnName] = source.[KeyColumnName]
                                AND target.[Source] = source.[Source]
                                AND target.[Type] = source.[Type]
                             ) THEN
            UPDATE SET [CleanedData] = source.[CleanedData],
                       [Key] = source.[Key], 
                       [KeyColumnName] = source.[KeyColumnName], 
                       [Source] = source.[Source],
                       [Type] = source.[Type]
        WHEN NOT MATCHED THEN
            INSERT
            (
                [RawData],
                   [CleanedData],
                   [Key],
                   [KeyColumnName],                   
                   [Source],
                   [Type]
            )
            VALUES
            (source.[RawData], source.[CleanedData], source.[Key], source.[KeyColumnName], source.[Source], source.[Type]);

        --REMOVE DUPLICATES, just in case
        ;WITH cte as
        (
            SELECT 
                Row_number() OVER(PARTITION BY  RawData, CleanedData, [Key] ORDER BY [Key]) RowNum, 
                RawData, 
                CleanedData,
                [Key],
                [KeyColumnName],
                [Source],
                [Type]
            FROM 
                [DeliusRunningPicture].[StandardisationReference] 
        )DELETE FROM cte WHERE RowNum > 1

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