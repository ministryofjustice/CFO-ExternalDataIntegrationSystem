using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StatusMessages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Globalization;

namespace DbInteractions.Services;

public class DbInteractionService : IDbInteractionService
{
    private static readonly string[] DeliusTables =
    [
        "AdditionalIdentifier", "AliasDetails", "Disability", "Disposal", "EventDetails", "Header",
        "MainOffence", "OAS", "OffenderAddress", "OffenderManager", "OffenderManagerBuildings",
        "OffenderManagerTeam", "OffenderToOffenderManagerMappings", "Offenders", "OffenderTransfer",
        "PersonalCircumstances", "Provision", "RegistrationDetails", "Requirement"
    ];

    private static readonly string[] OfflocTables =
    [
        "Activities", "Addresses", "Agencies", "Assessments", "Bookings", "Employment", "Flags",
        "Identifiers", "IncentiveLevel", "Locations", "MainOffence", "Movements", "OffenderAgencies",
        "OffenderStatus", "OtherOffences", "PersonalDetails", "PNC", "PreviousPrisonNumbers",
        "SentenceInformation", "SexOffenders"
    ];

    private static readonly CultureInfo EnGb = CultureInfo.GetCultureInfo("en-GB");

    private readonly IFileLocations fileLocations;
    private readonly ServerConfiguration serverConfig;
    private readonly IConfiguration configuration;
    private readonly IMessageService messageService;
    private readonly ILogger<DbInteractionService> logger;

    public DbInteractionService(
        IOptions<ServerConfiguration> serverConfig,
        IMessageService messageService,
        IConfiguration config,
        IFileLocations fileLocations,
        ILogger<DbInteractionService> logger)
    {
        this.serverConfig = serverConfig.Value;
        this.configuration = config;
        this.fileLocations = fileLocations;
        this.messageService = messageService;
        this.logger = logger;
    }

    private static async Task BulkLoadTableAsync(SqlConnection conn, string schema, string tableName, string basePath, ILogger logger)
    {
        var filePath = Path.Combine(basePath, $"{tableName}.txt");
        if (!File.Exists(filePath))
            return;

        var dataTable = new DataTable();
        using (var schemaCmd = new SqlCommand($"SELECT TOP 0 * FROM [{schema}].[{tableName}]", conn))
        using (var schemaReader = await schemaCmd.ExecuteReaderAsync())
        {
            dataTable.Load(schemaReader);
        }

        int lineNumber = 0;
        int parseFailures = 0;
        using (var fileReader = new StreamReader(filePath, detectEncodingFromByteOrderMarks: true))
        {
            string? line;
            while ((line = await fileReader.ReadLineAsync()) != null)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                try
                {
                    var fields = line.Split('|');
                    var row = dataTable.NewRow();
                    for (int i = 0; i < dataTable.Columns.Count && i < fields.Length; i++)
                        row[i] = ParseField(fields[i], dataTable.Columns[i].DataType);
                    dataTable.Rows.Add(row);
                }
                catch (Exception ex)
                {
                    parseFailures++;
                    logger.LogWarning(ex,
                        "Skipped unparseable row at line {LineNumber} in {Table} ({FilePath}): {Content}",
                        lineNumber, tableName, filePath,
                        line.Length > 500 ? line[..500] + "…" : line);
                }
            }
        }

        if (parseFailures > 0)
            logger.LogWarning("{ParseFailures} row(s) skipped in {Table} due to parse errors.", parseFailures, tableName);

        int rowCount = dataTable.Rows.Count;

        if (rowCount == 0)
        {
            logger.LogInformation("No rows to insert for {Schema}.{Table} — skipping.", schema, tableName);
            return;
        }

        logger.LogInformation("Inserting {RowCount} row(s) into {Schema}.{Table} from {FilePath}.", rowCount, schema, tableName, filePath);

        using var bulkCopy = new SqlBulkCopy(conn)
        {
            DestinationTableName = $"[{schema}].[{tableName}]",
            BulkCopyTimeout = 600
        };
        for (int i = 0; i < dataTable.Columns.Count; i++)
            bulkCopy.ColumnMappings.Add(i, i);

        try
        {
            await bulkCopy.WriteToServerAsync(dataTable);
            logger.LogInformation("Successfully inserted {RowCount} row(s) into {Schema}.{Table}.", rowCount, schema, tableName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Bulk insert failed for {Schema}.{Table}, falling back to row-by-row insertion.", schema, tableName);
            await InsertRowByRowAsync(conn, schema, tableName, dataTable, logger);
        }
    }

    private static async Task InsertRowByRowAsync(SqlConnection conn, string schema, string tableName, DataTable dataTable, ILogger logger)
    {
        var columns = dataTable.Columns.Cast<DataColumn>().ToList();
        var colList = string.Join(", ", columns.Select(c => $"[{c.ColumnName}]"));
        var paramList = string.Join(", ", columns.Select((_, i) => $"@p{i}"));
        var sql = $"INSERT INTO [{schema}].[{tableName}] ({colList}) VALUES ({paramList})";

        int total = dataTable.Rows.Count;
        int failures = 0;
        foreach (DataRow row in dataTable.Rows)
        {
            try
            {
                using var cmd = new SqlCommand(sql, conn);
                for (int i = 0; i < columns.Count; i++)
                    cmd.Parameters.AddWithValue($"@p{i}", row[i]);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                failures++;
                logger.LogWarning(ex, "Skipped row that failed to insert into {Schema}.{Table}.", schema, tableName);
            }
        }

        int inserted = total - failures;
        if (failures > 0)
            logger.LogWarning("{Failures} row(s) failed to insert into {Schema}.{Table} and were skipped. {Inserted}/{Total} rows inserted.", failures, schema, tableName, inserted, total);
        else
            logger.LogInformation("Row-by-row fallback: successfully inserted all {Total} row(s) into {Schema}.{Table}.", total, schema, tableName);
    }

    private static object ParseField(string value, Type targetType)
    {
        if (string.IsNullOrEmpty(value))
            return DBNull.Value;
        if (targetType == typeof(string))
            return value;
        if (targetType == typeof(long))
            return long.Parse(value, CultureInfo.InvariantCulture);
        if (targetType == typeof(int))
            return int.Parse(value, CultureInfo.InvariantCulture);
        if (targetType == typeof(DateTime))
            return DateTime.Parse(value, EnGb);
        if (targetType == typeof(byte[]))
            return Convert.FromHexString(value);
        return value;
    }

    public async Task<string[]> GetProcessedDeliusFileNames()
    {
        SqlConnection conn = new SqlConnection(configuration.GetConnectionString("DeliusRunningPictureDb")!);

        await conn.OpenAsync();

        using (conn)
        {
            SqlCommand cmd =
                new SqlCommand($"SELECT FileName FROM [DeliusRunningPicture].[ProcessedFiles]", conn);

            var reader = await cmd.ExecuteReaderAsync();

            List<string> fileNames = new List<string>();

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    fileNames.Add(reader.GetString(0));
                }
            }

            return fileNames.ToArray();
        }
    }

    public async Task<int> DeliusGetFileIdLastFull()
    {
        int fileId = 0;

        SqlConnection conn = new SqlConnection(configuration.GetConnectionString("DeliusRunningPictureDb")!);

        using (conn)
        {
            await conn.OpenAsync();

            SqlCommand cmd =
                new SqlCommand("SELECT TOP(1) FileId FROM [DeliusRunningPicture].[ProcessedFiles] WHERE FileName LIKE '%full%' ORDER BY [FileId] DESC", conn);

            try
            {
                var reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    await reader.ReadAsync();
                    fileId = reader.GetInt32(0);
                }
            }
            catch (SqlException e)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
                return 0;
            }
        }

        return fileId;
    }

    //Returning primitive types instead of strings should make DMS work better over time.
    public async Task<int[]> GetProcessedOfflocIds()
    {
        SqlConnection conn = new SqlConnection(configuration.GetConnectionString("OfflocRunningPictureDb")!);

        using (conn)
        {
            await conn.OpenAsync();

            SqlCommand cmd = new SqlCommand("SELECT FileId FROM [OfflocRunningPicture].[ProcessedFiles];", conn);
            var reader = await cmd.ExecuteReaderAsync();
            List<int> fileNames = new List<int>();

            while (await reader.ReadAsync())
            {
                fileNames.Add(reader.GetInt32(0));
            }

            return fileNames.ToArray();
        }
    }

    //private DateTime GetOfflocDate(string offlocFile)
    //{        
    //    string dateString = offlocFile.Substring(18, 6);
    //    dateString = $"{dateString[0..2]}/{dateString[2..4]}/{4..8}";

    //    DateTime result;
    //    DateTime.TryParse(dateString, new CultureInfo("en-GB"), out result);

    //    return result;
    //}

    public async Task<string[]> GetProcessedOfflocFileNames()
    {
        SqlConnection conn = new SqlConnection(configuration.GetConnectionString("OfflocRunningPictureDb")!);

        using (conn)
        {
            await conn.OpenAsync();

            SqlCommand cmd = new SqlCommand(@"
                SELECT FileName as [Name] FROM [OfflocRunningPicture].[ProcessedFiles] 
                UNION 
                SELECT ArchiveFileName as [Name] FROM [OfflocRunningPicture].[ProcessedFiles] WHERE ArchiveFileName IS NOT NULL", conn);

            var reader = await cmd.ExecuteReaderAsync();

            List<string> fileNames = new List<string>();
            while (await reader.ReadAsync())
            {
                fileNames.Add(reader.GetString(0));
            }

            return fileNames.ToArray();
        }
    }

    public async Task StageDelius(string fileName, string filePath)
    {
        string basePath = Path.Combine(fileLocations.deliusOutput, Path.GetFileNameWithoutExtension(fileName));

        await messageService.PublishAsync(new StatusUpdateMessage($"Delius staging started for file number {fileName}"));

        using var conn = new SqlConnection(configuration.GetConnectionString("DeliusStagingDb")!);
        await conn.OpenAsync();

        try
        {
            foreach (var table in DeliusTables)
                await BulkLoadTableAsync(conn, "DeliusStaging", table, basePath, logger);
        }
        catch (Exception e)
        {
            await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
            return;
        }

        try
        {
            SqlCommand cmd = new SqlCommand(serverConfig.DeliusStagingProcedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 600;
            cmd.Parameters.AddWithValue("@processedFile", fileName);
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
            return;
        }

        await messageService.PublishAsync(new StatusUpdateMessage($"Delius staging finished for file {fileName}."));
    }

    public async Task StageOffloc(string fileName)
    {
        string basePath = Path.Combine(fileLocations.offlocOutput, Path.GetFileNameWithoutExtension(fileName));

        await messageService.PublishAsync(new StatusUpdateMessage($"Offloc staging started for file {fileName}."));

        using var conn = new SqlConnection(configuration.GetConnectionString("OfflocStagingDb")!);
        await conn.OpenAsync();

        try
        {
            foreach (var table in OfflocTables)
                await BulkLoadTableAsync(conn, "OfflocStaging", table, basePath, logger);
        }
        catch (Exception e)
        {
            await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
            return;
        }

        try
        {
            SqlCommand command = new SqlCommand(serverConfig.OfflocStagingProcedure, conn);
            command.CommandTimeout = 600;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@processedFile", fileName);
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
            return;
        }

        await messageService.PublishAsync(new StatusUpdateMessage($"Offloc staging finished for file {fileName}."));
    }
    //Calls merge and then on completion
    public async Task StandardiseDeliusStaging()
    {
        SqlConnection conn = new SqlConnection(configuration.GetConnectionString("DeliusStagingDb")!);

        using (conn)
        {
            await conn.OpenAsync();

            SqlCommand command = new SqlCommand(serverConfig.DeliusStagingStandardiseDataProcedure, conn);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 600;

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException exception)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(exception.Message));
                return;
            }
        }
        await messageService.PublishAsync(new StatusUpdateMessage("Delius staging database standardisation complete."));
    }
    //Calls merge and then on completion
    public async Task MergeDeliusPicture(string fileName)
    {
        SqlConnection conn = new SqlConnection(configuration.GetConnectionString("DeliusRunningPictureDb")!);

        using (conn)
        {
            await conn.OpenAsync();

            SqlCommand command = new SqlCommand(serverConfig.DeliusRunningPictureMergeProcedure, conn);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 1200;

            command.Parameters.AddWithValue("@fileName", fileName);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException exception)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(exception.Message));
                return;
            }
        }
        await messageService.PublishAsync(new StatusUpdateMessage("Delius merging complete."));
    }

    public async Task ClearDeliusStaging()
    {
        SqlConnection conn = new SqlConnection(configuration.GetConnectionString("DeliusStagingDb")!);

        using (conn)
        {
            await conn.OpenAsync();

            SqlCommand command = new SqlCommand(serverConfig.DeliusClearStagingProcedure, conn);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 600;

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException exception)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(exception.Message));
                return;
            }
        }
        await messageService.PublishAsync(new StatusUpdateMessage("Delius staging database cleared."));
    }

    public async Task MergeOfflocPicture(string fileName)
    {
        SqlConnection offlocConn = new SqlConnection(configuration.GetConnectionString("OfflocRunningPictureDb")!);

        using (offlocConn)
        {
            await offlocConn.OpenAsync();

            SqlCommand command = new SqlCommand(serverConfig.OfflocRunningPictureMergeProcedure, offlocConn);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 1200;

            command.Parameters.AddWithValue("@fileName", fileName);

            try
            {
                var res = await command.ExecuteNonQueryAsync();
            }
            catch (SqlException e)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
                return;
            }
        }
        await messageService.PublishAsync(new StatusUpdateMessage("Offloc merging complete."));
    }

    public async Task ClearOfflocStaging()
    {
        SqlConnection offlocConn = new SqlConnection(configuration.GetConnectionString("OfflocStagingDb")!);
        using (offlocConn)
        {
            await offlocConn.OpenAsync();

            SqlCommand command = new SqlCommand(serverConfig.OfflocClearStagingProcedure, offlocConn);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 600;

            try
            {
                var res = await command.ExecuteNonQueryAsync();
            }
            catch (SqlException e)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
                return;
            }
        }

        await messageService.PublishAsync(new StatusUpdateMessage("Offloc staging database cleared."));
    }

    public async Task CreateOfflocProcessedFileEntry(string fileName, int fileId, string? archiveName = null)
    {
        SqlConnection offlocConn = new(configuration.GetConnectionString("OfflocRunningPictureDb")!);

        using (offlocConn)
        {
            await offlocConn.OpenAsync();

            var command = new SqlCommand(@"INSERT INTO [OfflocRunningPicture].[ProcessedFiles] (FileName, FileId, ArchiveFileName, Status) VALUES (@fileName, @fileId, @archiveName, @status)", offlocConn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 1200
            };
            command.Parameters.AddWithValue("@fileName", fileName);
            command.Parameters.AddWithValue("@fileId", fileId);
            command.Parameters.AddWithValue("@archiveName", (object?)archiveName ?? DBNull.Value);
            command.Parameters.AddWithValue("@status", "Processing");

            try
            {
                var res = await command.ExecuteNonQueryAsync();
            }
            catch (SqlException e)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
                return;
            }
        }
    }

    public async Task CreateDeliusProcessedFileEntry(string fileName, string fileId)
    {
        SqlConnection deliusConn = new(configuration.GetConnectionString("DeliusRunningPictureDb")!);

        using (deliusConn)
        {
            await deliusConn.OpenAsync();
            var command = new SqlCommand(@"INSERT INTO [DeliusRunningPicture].[ProcessedFiles] (FileName, FileId, Status) VALUES (@fileName, @fileId, @status)", deliusConn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 1200
            };

            command.Parameters.AddWithValue("@fileName", fileName);
            command.Parameters.AddWithValue("@fileId", fileId);
            command.Parameters.AddWithValue("@status", "Processing");

            try
            {
                var res = await command.ExecuteNonQueryAsync();
            }
            catch (SqlException e)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
                return;
            }
        }
    }

    public async Task AssociateOfflocFileWithArchive(string fileName, string archiveName)
    {
        SqlConnection offlocConn = new(configuration.GetConnectionString("OfflocRunningPictureDb")!);

        using (offlocConn)
        {
            await offlocConn.OpenAsync();

            var command = new SqlCommand(@"
                UPDATE [OfflocRunningPicture].[ProcessedFiles]
                SET ArchiveFileName = @archiveName
                WHERE FileName = @fileName", offlocConn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 1200
            };
            
            command.Parameters.AddWithValue("@fileName", fileName);
            command.Parameters.AddWithValue("@archiveName", archiveName);

            try
            {
                var res = await command.ExecuteNonQueryAsync();
            }
            catch (SqlException e)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
                return;
            }
        }
    }

    public async Task<bool> IsDeliusReadyForProcessing()
    {
        SqlConnection deliusConn = new(configuration.GetConnectionString("DeliusRunningPictureDb")!);

        using (deliusConn)
        {
            await deliusConn.OpenAsync();

            var command = new SqlCommand(@"
            SELECT CAST(IIF(
                NOT EXISTS (
                    SELECT 1 
                    FROM DeliusRunningPicture.ProcessedFiles 
                    WHERE Status <> 'Merged'
                ),
                1,
                0
            ) AS BIT)", deliusConn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 1200
            };

            try
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                bool result = (bool)await command.ExecuteScalarAsync();
#pragma warning restore CS8605 // Unboxing a possibly null value.
                return result;
            }
            catch (SqlException e)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
                return false;
            }
        }
    }

    public async Task<bool> IsOfflocReadyForProcessing()
    {
        SqlConnection offlocConn = new(configuration.GetConnectionString("OfflocRunningPictureDb")!);

        using (offlocConn)
        {
            await offlocConn.OpenAsync();

            var command = new SqlCommand(@"
            SELECT CAST(IIF(
                NOT EXISTS (
                    SELECT 1 
                    FROM OfflocRunningPicture.ProcessedFiles 
                    WHERE Status <> 'Merged'
                ),
                1,
                0
            ) AS BIT)", offlocConn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 1200
            };

            try
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                bool result = (bool)await command.ExecuteScalarAsync();
#pragma warning restore CS8605 // Unboxing a possibly null value.
                return result;
            }
            catch (SqlException e)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
                return false;
            }
        }
    }

    public async Task<string?> GetLastProcessedOfflocFileName()
    {
        SqlConnection offlocConn = new(configuration.GetConnectionString("OfflocRunningPictureDb")!);

        using (offlocConn)
        {
            await offlocConn.OpenAsync();

            var command = new SqlCommand(@"
            SELECT TOP 1 FileName 
            FROM OfflocRunningPicture.ProcessedFiles 
            ORDER BY ValidFrom DESC", offlocConn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 1200
            };

            try
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                string? result = (string?)await command.ExecuteScalarAsync();
#pragma warning restore CS8605 // Unboxing a possibly null value.
                return result;
            }
            catch (SqlException e)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
                return null;
            }
        }
    }

    public async Task<string?> GetLastProcessedDeliusFileName()
    {
        SqlConnection deliusConn = new(configuration.GetConnectionString("DeliusRunningPictureDb")!);

        using (deliusConn)
        {
            await deliusConn.OpenAsync();

            var command = new SqlCommand(@"
            SELECT TOP 1 FileName 
            FROM DeliusRunningPicture.ProcessedFiles 
            ORDER BY ValidFrom DESC", deliusConn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 1200
            };

            try
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                string? result = (string?)await command.ExecuteScalarAsync();
#pragma warning restore CS8605 // Unboxing a possibly null value.
                return result;
            }
            catch (SqlException e)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
                return null;
            }
        }
    }
}
