using System.Globalization;
using FileStorage;
using Messaging.Messages;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.MatchingMessages.Clustering;
using Messaging.Messages.StagingMessages;
using Messaging.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileSync;

public class FileSyncBackgroundService(
    ILogger<FileSyncBackgroundService> logger,
    IMatchingMessagingService matchingMessagingService,
    IDbMessagingService dbMessagingService,
	IStagingMessagingService stagingMessagingService,
    IFileLocations fileLocations,
    FileSource fileSource,
    IOptions<SyncOptions> syncOptions) : BackgroundService, IDisposable
{
    private readonly SemaphoreSlim gate = new(1, 1);
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
                        gate.Release();
                        await ProcessMessageAsync(msg, stoppingToken);
                    }, TMatchingQueue.ClusteringPostProcessingFinished);
            }

            if (syncOptions.Value is { ProcessOnTimer: true, ProcessTimerIntervalSeconds: > 0 })
            {
                timer = new Timer(
                    callback: async (state) => await ProcessAsync(stoppingToken),
                    state: null,
                    dueTime: Timeout.InfiniteTimeSpan,
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
        if (!await gate.WaitAsync(0, cancellationToken))
        {
            logger.LogWarning("Unable to process any new files, never received ClusteringPostProcessingFinishedMessage");
            return;
        }

        logger.LogInformation("Executing ProcessAsync...");

        var unprocessedDeliusFile = await GetNextUnprocessedDeliusFileAsync(cancellationToken);
        var unprocessedOfflocFile = await GetNextUnprocessedOfflocFileAsync(cancellationToken);

        // No files to process
        if (unprocessedDeliusFile is null || unprocessedOfflocFile is null)
        {
            return;
        }

        // Perform pre-kickoff tasks here?

        await fileSource.RetrieveFileAsync(unprocessedDeliusFile, fileLocations.deliusInput, cancellationToken);
        stagingMessagingService.StagingPublish(new DeliusDownloadFinishedMessage(Path.GetFileName(unprocessedDeliusFile)));

        await fileSource.RetrieveFileAsync(unprocessedOfflocFile, fileLocations.offlocInput, cancellationToken);
        stagingMessagingService.StagingPublish(new OfflocDownloadFinished(Path.GetFileName(unprocessedOfflocFile)));

    }

    private async Task<string?> GetNextUnprocessedOfflocFileAsync(CancellationToken cancellationToken = default)
    {
        // Get files from file store (s3 / local file system)
        var offlocFileStore = await fileSource.ListOfflocFilesAsync(cancellationToken);

        // Get already processed files
        var processedOfflocFiles = await GetAlreadyProcessedOfflocFilesAsync();

        // Exclude staged (downloaded) files
        var stagedOfflocFiles = Directory.GetFiles(fileLocations.offlocInput);
        processedOfflocFiles.AddRange(stagedOfflocFiles);

        // Return unprocessed files
        var unprocessedOfflocFile = offlocFileStore
            .ExceptBy(processedOfflocFiles, file => Path.GetFileName(file))
            .OrderBy(Datestamp).ThenBy(Index)
            .ToList()
            .FirstOrDefault();

        int Index(string fileName)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var parts = name.Split('_');
            return int.Parse(parts[4]);
        }

        DateOnly Datestamp(string fileName)
        {
            // C_NOMIS_OFFENDER_ddMMyyyy_01.dat
            var parts = fileName.Split('_');

            // 0. C
            // 1. NOMIS
            // 2. OFFENDER
            // 3. ddMMyyyy
            // 4. 01 (or 02, 03, 03, etc,.)
            var datePart = parts[3];

            return DateOnly.ParseExact(datePart, "ddMMyyyy", CultureInfo.InvariantCulture);
        }

        return unprocessedOfflocFile;
    }
    
    private async Task<string?> GetNextUnprocessedDeliusFileAsync(CancellationToken cancellationToken = default)
    {
        // Get files from file store (s3 / local file system)
        var deliusFileStore = await fileSource.ListDeliusFilesAsync(cancellationToken);
        
        // Get already processed files
        var processedDeliusFiles = await GetAlreadyProcessedDeliusFilesAsync();

        // Exclude staged (downloaded) files if using a non-filesystem source
        var stagedDeliusFiles = Directory.GetFiles(fileLocations.deliusInput);
        processedDeliusFiles.AddRange(stagedDeliusFiles);

        // Return unprocessed files
        var unprocessedDeliusFiles = deliusFileStore
            .ExceptBy(processedDeliusFiles, file => Path.GetFileName(file))
            .OrderBy(file => file)
            .ToList()
            .FirstOrDefault();
        
        return unprocessedDeliusFiles;
    }
    
    private async Task<List<string>> GetAlreadyProcessedOfflocFilesAsync()
    {
        logger.LogInformation("Getting processed Offloc files...");
        var offloc = await dbMessagingService.DbTransientSubscribe<GetOfflocFilesMessage, OfflocFilesReturnMessage>(new GetOfflocFilesMessage());
        logger.LogInformation("Get processed files done.");

        return offloc.offlocFiles.ToList();
    }

    private async Task<List<string>> GetAlreadyProcessedDeliusFilesAsync()
    {
        logger.LogInformation("Getting processed Delius files...");
        var delius = await dbMessagingService.DbTransientSubscribe<GetDeliusFilesMessage, DeliusFilesReturnMessage>(new GetDeliusFilesMessage());
        logger.LogInformation("Get processed files done.");

        return delius.fileNames.ToList();
    }
    
    public override void Dispose()
    {
        timer?.Dispose();
    }
    
}
