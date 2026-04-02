using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StatusMessages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Data;
using System.Globalization;

namespace DbInteractions.Services;

public class DbInteractionService : IDbInteractionService
{
    private readonly IFileLocations fileLocations;
    private readonly ServerConfiguration serverConfig;
    private readonly IConfiguration configuration;
    private readonly bool inContainer;
    private readonly IMessageService messageService;

    public DbInteractionService(
        IOptions<ServerConfiguration> serverConfig,
        IMessageService messageService,
        IConfiguration config,
        IFileLocations fileLocations)
    {
        this.serverConfig = serverConfig.Value;
        this.configuration = config;
        this.fileLocations = fileLocations;
        this.messageService = messageService;
        this.inContainer = config.GetValue<bool>("RUNNING_IN_CONTAINER");
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
        string folderName = filePath.Split('/').Last();

        await messageService.PublishAsync(new StatusUpdateMessage($"Delius staging started for file number {fileName}"));

        string containerFlag = string.Empty;
        if (inContainer)
        {
            containerFlag = "Y";
        }
        else
        {
            containerFlag = "N";
        }

        var deliusConn = new SqlConnection(configuration.GetConnectionString("DeliusStagingDb")!);
        using (deliusConn)
        {
            await deliusConn.OpenAsync();

            SqlCommand cmd = new SqlCommand(serverConfig.DeliusStagingProcedure, deliusConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 600;
            cmd.Parameters.AddWithValue("@basePath", $"{fileLocations.deliusOutput}/{folderName}/");
            cmd.Parameters.AddWithValue("@processedFile", fileName);
            cmd.Parameters.AddWithValue("@inContainer", containerFlag);

            try
            {
                var res = await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                await messageService.PublishAsync(new StatusUpdateMessage(e.Message));
                return;
            }
        }

        await messageService.PublishAsync(new StatusUpdateMessage($"Delius staging finished for file {fileName}."));
    }

    public async Task StageOffloc(string fileName)
    {
        string folderName = fileName.Split('.').First();
        string folderPath = Path.Combine(fileLocations.offlocOutput, folderName);

        await messageService.PublishAsync(new StatusUpdateMessage($"Offloc staging started for file {fileName}."));

        var tableNames = new[]
        {
            "Activities", "Addresses", "Agencies", "Assessments", "Bookings",
            "Employment", "Flags", "Identifiers", "IncentiveLevel", "Locations",
            "MainOffence", "Movements", "OffenderAgencies", "OffenderStatus",
            "OtherOffences", "PersonalDetails", "PNC", "PreviousPrisonNumbers",
            "SentenceInformation", "SexOffenders"
        };

        var offlocConn = new SqlConnection(configuration.GetConnectionString("OfflocStagingDb")!);
        using (offlocConn)
        {
            await offlocConn.OpenAsync();

            try
            {
                foreach (var table in tableNames)
                {
                    var filePath = Path.Combine(folderPath, $"{table}.txt");
                    if (!File.Exists(filePath)) continue;

                    var columns = await GetTableColumns(offlocConn, "OfflocStaging", table);
                    var dataTable = ReadPipeDelimitedFile(filePath, columns);

                    using var bulkCopy = new SqlBulkCopy(offlocConn)
                    {
                        DestinationTableName = $"[OfflocStaging].[{table}]",
                        BulkCopyTimeout = 600
                    };
                    await bulkCopy.WriteToServerAsync(dataTable);
                }

                // Calls StandardiseData + updates ProcessedFiles
                SqlCommand command = new SqlCommand(serverConfig.OfflocStagingProcedure, offlocConn);
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
        }

        await messageService.PublishAsync(new StatusUpdateMessage($"Offloc Staging completed."));
    }

    private async Task<List<(string Name, int? MaxLength, string DataType)>> GetTableColumns(SqlConnection conn, string schema, string table)
    {
        var columns = new List<(string Name, int? MaxLength, string DataType)>();
        using var cmd = new SqlCommand(
            "SELECT COLUMN_NAME, CHARACTER_MAXIMUM_LENGTH, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table ORDER BY ORDINAL_POSITION",
            conn);
        cmd.Parameters.AddWithValue("@schema", schema);
        cmd.Parameters.AddWithValue("@table", table);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var name = reader.GetString(0);
            var maxLength = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);
            var dataType = reader.GetString(2);
            columns.Add((name, maxLength, dataType));
        }
        return columns;
    }

    private static readonly string[] DateTypes = ["date", "datetime", "datetime2", "smalldatetime"];
    private static readonly string[] IntTypes = ["int", "smallint", "tinyint", "bigint"];
    private static readonly CultureInfo UkCulture = new CultureInfo("en-GB");

    private DataTable ReadPipeDelimitedFile(string filePath, List<(string Name, int? MaxLength, string DataType)> columns)
    {
        var dataTable = new DataTable();
        foreach (var col in columns)
        {
            if (DateTypes.Contains(col.DataType))
                dataTable.Columns.Add(col.Name, typeof(DateTime)).AllowDBNull = true;
            else if (IntTypes.Contains(col.DataType))
                dataTable.Columns.Add(col.Name, typeof(int)).AllowDBNull = true;
            else
                dataTable.Columns.Add(col.Name, typeof(string));
        }

        foreach (var line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var values = line.Split('|');
            var row = dataTable.NewRow();
            for (int i = 0; i < Math.Min(values.Length, columns.Count); i++)
            {
                var value = values[i];
                var col = columns[i];

                if (string.IsNullOrWhiteSpace(value))
                {
                    row[i] = DateTypes.Contains(col.DataType) || IntTypes.Contains(col.DataType)
                        ? DBNull.Value
                        : (object)"";
                    continue;
                }

                if (DateTypes.Contains(col.DataType))
                {
                    row[i] = DateTime.TryParse(value, UkCulture, DateTimeStyles.None, out var dt)
                        ? dt
                        : DBNull.Value;
                }
                else if (IntTypes.Contains(col.DataType))
                {
                    row[i] = int.TryParse(value, out var n) ? n : DBNull.Value;
                }
                else
                {
                    var maxLength = col.MaxLength;
                    if (maxLength.HasValue && maxLength.Value > 0 && value.Length > maxLength.Value)
                        value = value[..maxLength.Value];
                    row[i] = value;
                }
            }
            dataTable.Rows.Add(row);
        }
        return dataTable;
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
