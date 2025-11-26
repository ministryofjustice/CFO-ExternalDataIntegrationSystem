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
            await Task.Run(async () =>
            {
                await matchingMessagingService.MatchingSubscribeAsync<MatchingScoreCandidatesFinishedMessage>(async (message) =>
                {
                    await PreProcessAsync(stoppingToken);

                }, TMatchingQueue.MatchingScoreCandidatesFinished);

                await matchingMessagingService.MatchingSubscribeAsync<ClusteringPreProcessingFinishedMessage>(async (message) =>
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
        await matchingMessagingService.MatchingPublishAsync(new ClusteringPreProcessingStartedMessage());
    }

    private async Task PostProcessAsync(CancellationToken stoppingToken)
    {
        await statusMessagingService.StatusPublishAsync(new StatusUpdateMessage("Clustering (post-processing) started..."));

        await clusteringRepository.ClusterPostProcessAsync();
        await matchingMessagingService.MatchingPublishAsync(new ClusteringPostProcessingFinishedMessage());
    }

}
