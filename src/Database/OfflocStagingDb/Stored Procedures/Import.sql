-- =============================================
-- Author:		Daniel Fenelon
-- Create date: 18/03/2024
-- Description:	Import of the OffLoc files
-- =============================================

CREATE PROCEDURE [OfflocStaging].[Import]
    @basePath VARCHAR(100),
    @processedFile VARCHAR(50)
AS
BEGIN

    SET NOCOUNT ON;
    SET DATEFORMAT DMY;
    
    CREATE TABLE #TableNames
    (
        Id INT PRIMARY KEY IDENTITY,
        fileName VARCHAR(50) NOT NULL
    );
    INSERT INTO #TableNames
    (
        fileName
    )
    VALUES
    ('Activities'),
    ('Addresses'),
    ('Agencies'),
    ('Assessments'),
    ('Bookings'),
    ('Employment'),
    ('Flags'),
    ('Identifiers'),
    ('IncentiveLevel'),
    ('Locations'),
    ('MainOffence'),
    ('Movements'),
    ('OffenderAgencies'),
    ('OffenderStatus'),
    ('OtherOffences'),
    ('PersonalDetails'),
    ('PNC'),
    ('PreviousPrisonNumbers'),
    ('SentenceInformation'),
    ('SexOffenders');

    DECLARE @i INT;
    DECLARE @sql VARCHAR(250);
    DECLARE @fileName VARCHAR(150);

    SET @i = 1;

    BEGIN TRANSACTION;
    BEGIN TRY
        WHILE @i <= (SELECT COUNT(*)FROM #TableNames)
        BEGIN
            SET @fileName =
            (
                SELECT fileName FROM #TableNames WHERE Id = @i
            );
            SET @sql
                = N'BULK INSERT OfflocStaging.' + @fileName + N' FROM ''' + @basePath + @fileName + '.txt'''
                  + N' WITH (FieldTerminator = ''|'', RowTerminator = ''0x0d0a'', MAXERRORS = 1000)';
            EXEC (@sql);

            SET @i = @i + 1;
        END;
        DECLARE @stringDate CHAR(8);
        SET @stringDate = SUBSTRING(@processedFile, 18, 8);

        --Standardise Offloc Data
        Declare @retMessage varchar(500);
        EXEC [OfflocStaging].[StandardiseData] @retMessage;

        UPDATE [OfflocRunningPictureDb].[OfflocRunningPicture].[ProcessedFiles]
        SET [Status] = 'Imported'
        WHERE FileName = @processedFile;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;