using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StatusMessages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DbInteractions.Services;

public class DbInteractionService : IDbInteractionService
{
    private IStatusMessagingService statusService;
    private IFileLocations fileLocations;

    private ServerConfiguration serverConfig;

    private string deliusStagingConnString;
    private string offlocStagingConnString;
    private string deliusPictureConnString;
    private string offlocPictureConnString;

    private bool inContainer;

    public DbInteractionService(IStatusMessagingService messageService,
        ConnectionStrings connStrings, ServerConfiguration serverConfig,
        IConfiguration config, IFileLocations fileLocations)
    {
        this.statusService = messageService;
        this.serverConfig = serverConfig;
        this.fileLocations = fileLocations;

        deliusStagingConnString = connStrings.deliusStagingConnectionString;
        offlocStagingConnString = connStrings.offlocStagingConnectionString;
        deliusPictureConnString = connStrings.deliusPictureConnectionString;
        offlocPictureConnString = connStrings.offlocPictureConnectionString;

        //Make tidier.
        inContainer = config.GetValue<bool>("RUNNING_IN_CONTAINER");
    }

    public async Task<string[]> GetProcessedDeliusFileNames()
    {
        SqlConnection conn = new SqlConnection(deliusPictureConnString);

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

        SqlConnection conn = new SqlConnection(deliusPictureConnString);

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
                statusService.StatusPublish(new StatusUpdateMessage(e.Message));
                return 0;
            }
        }

        return fileId;
    }

    //Returning primitive types instead of strings should make DMS work better over time.
    public async Task<int[]> GetProcessedOfflocIds()
    {
        SqlConnection conn = new SqlConnection(offlocPictureConnString);

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
        SqlConnection conn = new SqlConnection(offlocPictureConnString);

        using (conn)
        {
            await conn.OpenAsync();

            SqlCommand cmd = new SqlCommand(@"
                SELECT FileName as [Name] FROM [OfflocRunningPicture].[ProcessedFiles] 
                UNION 
                SELECT ArchiveFileName as [Name] FROM [OfflocRunningPicture].[ProcessedFiles]", conn);

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

        statusService.StatusPublish(new StatusUpdateMessage($"Delius staging started for file number {fileName}"));

        string containerFlag = string.Empty;
        if (inContainer)
        {
            containerFlag = "Y";
        }
        else
        {
            containerFlag = "N";
        }

        var deliusConn = new SqlConnection(deliusStagingConnString);
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
                statusService.StatusPublish(new StatusUpdateMessage(e.Message));
                return;
            }
        }

        statusService.StatusPublish(new StatusUpdateMessage($"Delius staging finished for file {fileName}."));
    }

    public async Task StageOffloc(string fileName)
    {
        string folderName = fileName.Split('.').First();
        statusService.StatusPublish(new StatusUpdateMessage($"Offloc staging started for file {fileName}."));

        var offlocConn = new SqlConnection(offlocStagingConnString);
        using (offlocConn)
        {
            await offlocConn.OpenAsync();
            SqlCommand command = new SqlCommand(serverConfig.OfflocStagingProcedure, offlocConn);
            command.CommandTimeout = 600;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@basePath", $"{fileLocations.offlocOutput}/{folderName}/");
            command.Parameters.AddWithValue("@processedFile", fileName);

            try
            {
                var result = await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                statusService.StatusPublish(new StatusUpdateMessage(e.Message));
                return;
            }
        }

        statusService.StatusPublish(new StatusUpdateMessage($"Offloc staging finished for file {fileName}."));
    }
    //Calls merge and then on completion
    public async Task StandardiseDeliusStaging()
    {
        SqlConnection conn = new SqlConnection(deliusStagingConnString);

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
                statusService.StatusPublish(new StatusUpdateMessage(exception.Message));
                return;
            }
        }
        statusService.StatusPublish(new StatusUpdateMessage("Delius staging database standardisation complete."));
    }
    //Calls merge and then on completion
    public async Task MergeDeliusPicture(string fileName)
    {
        SqlConnection conn = new SqlConnection(deliusPictureConnString);

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
                statusService.StatusPublish(new StatusUpdateMessage(exception.Message));
                return;
            }
        }
        statusService.StatusPublish(new StatusUpdateMessage("Delius merging complete."));
    }

    public async Task ClearDeliusStaging()
    {
        SqlConnection conn = new SqlConnection(deliusStagingConnString);

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
                statusService.StatusPublish(new StatusUpdateMessage(exception.Message));
                return;
            }
        }
        statusService.StatusPublish(new StatusUpdateMessage("Delius staging database cleared."));
    }

    public async Task MergeOfflocPicture(string fileName)
    {
        SqlConnection offlocConn = new SqlConnection(offlocPictureConnString);

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
                statusService.StatusPublish(new StatusUpdateMessage(e.Message));
                return;
            }
        }
        statusService.StatusPublish(new StatusUpdateMessage("Offloc merging complete."));
    }

    public async Task ClearOfflocStaging()
    {
        SqlConnection offlocConn = new SqlConnection(offlocStagingConnString);
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
                statusService.StatusPublish(new StatusUpdateMessage(e.Message));
                return;
            }
        }

        statusService.StatusPublish(new StatusUpdateMessage("Offloc staging database cleared."));
    }

    public async Task CreateOfflocProcessedFileEntry(string fileName, int fileId, string? archiveName = null)
    {
        SqlConnection offlocConn = new(offlocPictureConnString);

        using (offlocConn)
        {
            await offlocConn.OpenAsync();

            var command = new SqlCommand(@"
                INSERT INTO [OfflocRunningPicture].[ProcessedFiles] (FileName, FileId, ArchiveFileName, Status) VALUES (@fileName, @fileId, @archiveName, @status)", offlocConn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 1200
            };

            command.Parameters.AddWithValue("@fileName", fileName);
            command.Parameters.AddWithValue("@fileId", fileId);
            command.Parameters.AddWithValue("@archiveName", archiveName);
            command.Parameters.AddWithValue("@status", "Processing");

            try
            {
                var res = await command.ExecuteNonQueryAsync();
            }
            catch (SqlException e)
            {
                statusService.StatusPublish(new StatusUpdateMessage(e.Message));
                return;
            }
        }
    }

    public async Task CreateDeliusProcessedFileEntry(string fileName, string fileId)
    {
        SqlConnection deliusConn = new(deliusPictureConnString);

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
                statusService.StatusPublish(new StatusUpdateMessage(e.Message));
                return;
            }
        }
    }

    public async Task AssociateOfflocFileWithArchive(string fileName, string archiveName)
    {
        SqlConnection offlocConn = new(offlocPictureConnString);

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
                statusService.StatusPublish(new StatusUpdateMessage(e.Message));
                return;
            }
        }
    }
}
