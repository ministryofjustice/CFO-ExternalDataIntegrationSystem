using System.Globalization;
using System.IO.Compression;
using System.Text.RegularExpressions;
using FileStorage;
using FileSync.Extensions;
using Messaging.Messages;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.MatchingMessages.Clustering;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StagingMessages.Delius;
using Messaging.Messages.StagingMessages.Offloc;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileSync;

public class FileSyncBackgroundService(
    ILogger<FileSyncBackgroundService> logger,
    IMatchingMessagingService matchingMessagingService,
    IDbMessagingService dbMessagingService,
	IStagingMessagingService stagingMessagingService,
    IStatusMessagingService statusMessagingService,
    IFileLocations fileLocations,
    FileSource fileSource,
    IOptions<SyncOptions> syncOptions) : BackgroundService, IDisposable
{
    private Timer? timer = null;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting service...");

        try
        {
            if (syncOptions.Value.ProcessOnCompletion)
            {
                matchingMessagingService.MatchingSubscribe<ClusteringPostProcessingFinishedMessage>(
                    async msg =>
                    {
                        await ProcessMessageAsync(msg, stoppingToken);
                    }, TMatchingQueue.ClusteringPostProcessingFinished);
            }

            if (syncOptions.Value is { ProcessOnTimer: true, ProcessTimerIntervalSeconds: > 0 })
            {
                timer = new Timer(
                    callback: async (state) => await ProcessAsync(stoppingToken),
                    state: null,
                    dueTime: TimeSpan.FromSeconds(syncOptions.Value.ProcessTimerIntervalSeconds),
                    period: TimeSpan.FromSeconds(syncOptions.Value.ProcessTimerIntervalSeconds));
            }

            if (syncOptions.Value.ProcessOnStartup)
            {
                await ProcessAsync(stoppingToken);
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

    async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
    {
        logger.LogInformation($"{message.GetType().Name} received.");
        await ProcessAsync(cancellationToken);
    }

    async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var isDeliusReady = await IsDeliusReady();
        var isOfflocReady = await IsOfflocReady();

        var notReady = (isDeliusReady, isOfflocReady) switch
        {
            (false, false) => "Delius and Offloc are not ready for processing.",
            (false, true) => "Delius is not ready for processing.",
            (true, false) => "Offloc is not ready for processing.",
            _ => null
        };

        if (!string.IsNullOrEmpty(notReady))
        {               
            logger.LogWarning($"{notReady} Exiting.");
            return;
        }

        await PreKickoffTasks();

        var unprocessedDeliusFile = await GetNextUnprocessedDeliusFileAsync(cancellationToken);
        var unprocessedOfflocFile = await GetNextUnprocessedOfflocFileAsync(cancellationToken);

        // No files to process
        if (unprocessedDeliusFile is null || unprocessedOfflocFile is null)
        {
            return;
        }

        stagingMessagingService.StagingPublish(new DeliusDownloadFinishedMessage(unprocessedDeliusFile.Name, unprocessedDeliusFile.GetFileId()));
        stagingMessagingService.StagingPublish(new OfflocDownloadFinished(unprocessedOfflocFile.Name, unprocessedOfflocFile.GetFileId()!.Value, unprocessedOfflocFile.ParentArchiveName));
    }

    private async Task<bool> IsOfflocReady()
    {
        var response = await dbMessagingService.SendDbRequestAndWaitForResponse<IsOfflocReadyForProcessingMessage, IsOfflocReadyForProcessingReturnMessage>(new());
        return response.isReady;
    }

    private async Task<bool> IsDeliusReady()
    {
        var response = await dbMessagingService.SendDbRequestAndWaitForResponse<IsDeliusReadyForProcessingMessage, IsDeliusReadyForProcessingReturnMessage>(new());
        return response.isReady;
    }
    
    private async Task<OfflocFile?> GetNextUnprocessedOfflocFileAsync(CancellationToken cancellationToken = default)
    {
        // Get files from file store (s3 / minio / local file system)
        logger.LogInformation($"Retrieving Offloc files from {fileSource.GetType().Name} file store...");
        var offlocFileStore = await fileSource.ListOfflocFilesAsync(cancellationToken);
        logger.LogInformation($"Found {offlocFileStore.Count} Offloc file(s) in {fileSource.GetType().Name} file store.");

        // Get already processed files
        logger.LogInformation("Retrieving processed Offloc files...");
        var response = await dbMessagingService.SendDbRequestAndWaitForResponse<GetOfflocFilesMessage, OfflocFilesReturnMessage>(new GetOfflocFilesMessage());
        logger.LogInformation($"Retrieved {response.offlocFiles.Length} processed Offloc file(s).");

        // Find latest unprocessed file
        var unprocessedOfflocFiles = offlocFileStore
            .ExceptBy(response.offlocFiles, file => Path.GetFileName(file.Name)) // Excludes processed
            .OrderBy(f => f.GetDatestamp())
            .ToList();

        logger.LogInformation($"Found {unprocessedOfflocFiles.Count} unprocessed Offloc file(s).");

        var unprocessedOfflocFile = unprocessedOfflocFiles.FirstOrDefault();

        // Not files to process
        if(unprocessedOfflocFile is null)
        {
            return null;
        }

        // Download the file
        var downloadedFile = await fileSource.RetrieveFileAsync(unprocessedOfflocFile.Path, fileLocations.offlocInput, cancellationToken);

        // If the downloaded file is not an archive, process it.
        if(unprocessedOfflocFile.IsArchive is false)
        {
            logger.LogInformation("Target Offloc file: " + unprocessedOfflocFile.Name);
            return unprocessedOfflocFile;
        }

        logger.LogInformation("Target Offloc archive: " + unprocessedOfflocFile.Name);

        // If the downloaded file is an archive, extract it and check the contained file.
        // We only support archives containing a single Offloc (.dat) file.
        logger.LogInformation("Extracting Offloc archive: " + unprocessedOfflocFile.Name);
        await ZipFile.ExtractToDirectoryAsync(downloadedFile, fileLocations.offlocInput, cancellationToken);
        File.Delete(downloadedFile);

        var file = Directory.GetFiles(fileLocations.offlocInput)
            .Select(filePath => Path.GetFileName(filePath))
            .Where(fileName => Regex.IsMatch(fileName, FileConstants.OfflocFilePattern))
            .Single();

        logger.LogInformation("Extracted Offloc file: " + file);

        // We have already processed the contents of the archive, get the next unprocessed file.
        if(response.offlocFiles.Contains(file))
        {
            // Associate the zip with the file name
            logger.LogWarning("Already processed contents of archive: " + file);
            await dbMessagingService.SendDbRequestAndWaitForResponse<AssociateOfflocFileWithArchiveMessage, ResultAssociateOfflocFileWithArchiveMessage>(new AssociateOfflocFileWithArchiveMessage(file, Path.GetFileName(downloadedFile)));
            return await GetNextUnprocessedOfflocFileAsync(cancellationToken);
        }

        return new OfflocFile(Path.Combine(fileLocations.offlocInput, file), Path.GetFileName(downloadedFile));
    }
    
    private async Task<DeliusFile?> GetNextUnprocessedDeliusFileAsync(CancellationToken cancellationToken = default)
    {
        // Get files from file store (s3 / local file system)
        var deliusFileStore = await fileSource.ListDeliusFilesAsync(cancellationToken);
        
        // Get already processed files
        logger.LogInformation("Retrieving processed Delius files...");
        var response = await dbMessagingService.SendDbRequestAndWaitForResponse<GetDeliusFilesMessage, DeliusFilesReturnMessage>(new GetDeliusFilesMessage());
        logger.LogInformation($"Retrieved {response.fileNames.Length} processed Delius file(s).");

        // Return unprocessed files
        var unprocessedDeliusFiles = deliusFileStore
            .ExceptBy(response.fileNames, file => Path.GetFileName(file.Name))
            .OrderBy(file => file.GetDatestamp())
            .ToList();

        logger.LogInformation($"Found {unprocessedDeliusFiles.Count} unprocessed Delius file(s).");

        var unprocessedDeliusFile = unprocessedDeliusFiles.FirstOrDefault();

        if(unprocessedDeliusFile is null)
        {
            return null;
        }

        var downloadedFile = await fileSource.RetrieveFileAsync(unprocessedDeliusFile.Path, fileLocations.deliusInput, cancellationToken);
        
        logger.LogInformation("Target Delius file: " + unprocessedDeliusFile.Name);

        return new DeliusFile(Path.GetFileName(downloadedFile));
    }
    
    private async Task PreKickoffTasks()
    {
        LogStatus("Publishing pre-kickoff messages...");
        stagingMessagingService.StagingPublish(new ClearHalfCleanedOfflocFiles());
        stagingMessagingService.StagingPublish(new ClearTemporaryDeliusFiles());

        LogStatus("Pre-kickoff messages published. Beginning staging database tear down...");
        await dbMessagingService.SendDbRequestAndWaitForResponse<ClearDeliusStaging, ResultClearDeliusStaging>(new ClearDeliusStaging());
        await dbMessagingService.SendDbRequestAndWaitForResponse<ClearOfflocStaging, ResultClearOfflocStaging>(new ClearOfflocStaging());
        LogStatus("Staging database tear down complete.");
    }

    void LogStatus(string message)
    {
        logger.LogInformation(message);
        statusMessagingService.StatusPublish(new StatusUpdateMessage(message));
    }

    public override void Dispose()
    {
        timer?.Dispose();
    }
}
