using System.Diagnostics;
using FileStorage;
using Messaging.Messages;
using Messaging.Messages.MatchingMessages.Clustering;
using Messaging.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileSync;

public class FileSyncBackgroundService(
    ILogger<FileSyncBackgroundService> logger,
    IMatchingMessagingService matchingMessagingService,
    IDbMessagingService dbMessagingService,
    IFileLocations fileLocations,
    // IAmazonS3 client,
    IOptions<S3Options> s3Options,
    IOptions<SyncOptions> syncOptions) : BackgroundService, IDisposable
{
    private Timer? timer = null;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting service...");

        try
        {
            if (syncOptions.Value.ProcessOnStartup)
            {
                await ProcessAsync();
            }

            if (syncOptions.Value.ProcessOnCompletion)
            {
                matchingMessagingService.MatchingSubscribe<ClusteringPostProcessingFinishedMessage>(ProcessMessageAsync, TMatchingQueue.ClusteringPostProcessingFinished);
            }

            if (syncOptions.Value is { ProcessOnTimer: true, ProcessTimerIntervalSeconds: > 0 })
            {
                timer = new Timer(
                    callback: async c => await ProcessAsync(),
                    state: null,
                    dueTime: 0,
                    period: syncOptions.Value.ProcessTimerIntervalSeconds);
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }

        logger.LogInformation("Execution complete.");
    }

    async void ProcessMessageAsync(Message message)
    {
        logger.LogInformation($"{message.GetType().Name} received.");
        await ProcessAsync();
    }

    async Task ProcessAsync()
    {
        logger.LogInformation("Processing...");
    }
    
    public void Dispose()
    {
        timer?.Dispose();
    }
    
}
