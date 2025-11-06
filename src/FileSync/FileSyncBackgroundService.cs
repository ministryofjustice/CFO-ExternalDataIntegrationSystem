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
    FileSourceOptions fileSourceOptions,
    IOptions<SyncOptions> syncOptions) : BackgroundService, IDisposable
{
    private readonly SemaphoreSlim gate = new(1, 1);
    private Timer? timer = null;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting service...");

        try
        {
            if (syncOptions.Value.ProcessOnStartup)
            {
                await ProcessAsync(stoppingToken);
            }

            if (syncOptions.Value.ProcessOnCompletion)
            {
                matchingMessagingService.MatchingSubscribe<ClusteringPostProcessingFinishedMessage>(
                    async msg => await ProcessMessageAsync(msg, stoppingToken), TMatchingQueue.ClusteringPostProcessingFinished);
            }

            if (syncOptions.Value is { ProcessOnTimer: true, ProcessTimerIntervalSeconds: > 0 })
            {
                timer = new Timer(
                    callback: async (state) => await ProcessAsync(stoppingToken),
                    state: null,
                    dueTime: Timeout.InfiniteTimeSpan,
                    period: TimeSpan.FromSeconds(syncOptions.Value.ProcessTimerIntervalSeconds));
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
            return;

        try
        {
            logger.LogInformation("Executing ProcessAsync...");

            var unprocessedDeliusFiles = await GetUnprocessedDeliusFilesAsync(cancellationToken);
            var unprocessedOfflocFiles = await GetUnprocessedOfflocFilesAsync(cancellationToken);

            // No files to process
            if (unprocessedDeliusFiles is not { Count: > 0 } && unprocessedOfflocFiles is not { Count: > 0 })
            {
                return;
            }

            // Perform pre-kickoff tasks here

            foreach (var unprocessedDeliusFile in unprocessedDeliusFiles)
            {
                await fileSource.RetrieveFileAsync(unprocessedDeliusFile, fileLocations.deliusInput, cancellationToken);
                // stagingMessagingService.StagingPublish(new DeliusDownloadFinishedMessage(unprocessedDeliusFile));
                stagingMessagingService.StagingPublish(new DeliusDownloadFinishedMessage());
            }

            foreach (var unprocessedOfflocFile in unprocessedOfflocFiles)
            {
                await fileSource.RetrieveFileAsync(unprocessedOfflocFile, fileLocations.offlocInput, cancellationToken);
                // stagingMessagingService.StagingPublish(new OfflocDownloadFinished(unprocessedOfflocFile));
                stagingMessagingService.StagingPublish(new OfflocDownloadFinished());
            }
            
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<IReadOnlyList<string>> GetUnprocessedOfflocFilesAsync(CancellationToken cancellationToken = default)
    {
        // Get files from file store (s3 / local file system)
        var offlocFileStore = await fileSource.ListOfflocFilesAsync(fileSourceOptions.Source, cancellationToken);

        // Get already processed files
        var processedOfflocFiles = await GetAlreadyProcessedOfflocFilesAsync();

        // Exclude staged (downloaded) files
        var stagedOfflocFiles = Directory.GetFiles(fileLocations.offlocInput);
        processedOfflocFiles.AddRange(stagedOfflocFiles);

        // Return unprocessed files
        var unprocessedOfflocFiles = offlocFileStore.Select(file => Path.GetFileName(file)!).Except(processedOfflocFiles).ToList();
        
        // TODO: Need to order
        return unprocessedOfflocFiles.AsReadOnly();
    }
    
    private async Task<IReadOnlyList<string>> GetUnprocessedDeliusFilesAsync(CancellationToken cancellationToken = default)
    {
        // Get files from file store (s3 / local file system)
        var deliusFileStore = await fileSource.ListDeliusFilesAsync(fileSourceOptions.Source, cancellationToken);
        
        // Get already processed files
        var processedDeliusFiles = await GetAlreadyProcessedDeliusFilesAsync();

        // Exclude staged (downloaded) files if using a non-filesystem source
        var stagedDeliusFiles = Directory.GetFiles(fileLocations.deliusInput);
        processedDeliusFiles.AddRange(stagedDeliusFiles);

        // Return unprocessed files
        var unprocessedDeliusFiles = deliusFileStore.Select(file => Path.GetFileName(file)!).Except(processedDeliusFiles).ToList();
        
        // TODO: Need to order
        return unprocessedDeliusFiles.AsReadOnly();
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
