using Messaging.Interfaces;
using Messaging.Messages.MatchingMessages;
using Messaging.Messages.MatchingMessages.Clustering;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Matching.Engine.Services;

public class ClusteringService(
    IStatusMessagingService statusMessagingService,
    IMatchingMessagingService matchingMessagingService, 
    IClusteringRepository clusteringRepository,
    ILogger<ClusteringService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Run(() =>
            {
                matchingMessagingService.MatchingSubscribe<MatchingScoreCandidatesFinishedMessage>(async (message) =>
                {
                    await PreProcessAsync(stoppingToken);

                }, TMatchingQueue.MatchingScoreCandidatesFinished);

                matchingMessagingService.MatchingSubscribe<ClusteringPreProcessingFinishedMessage>(async (message) =>
                {
                    await PostProcessAsync(stoppingToken);
                }, TMatchingQueue.ClusteringPreProcessingFinished);

            }, stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred");
        }
    }

    private async Task PreProcessAsync(CancellationToken stoppingToken)
    {
        await clusteringRepository.ClusterPreProcessAsync();
        matchingMessagingService.MatchingPublish(new ClusteringPreProcessingStartedMessage());
    }

    private async Task PostProcessAsync(CancellationToken stoppingToken)
    {
        statusMessagingService.StatusPublish(new StatusUpdateMessage("Clustering (post-processing) started..."));

        await clusteringRepository.ClusterPostProcessAsync();
        matchingMessagingService.MatchingPublish(new ClusteringPostProcessingFinishedMessage());
    }

}
