using Messaging.Interfaces;
using Messaging.Messages.MatchingMessages;
using Messaging.Messages.MatchingMessages.Clustering;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Matching.Engine.Services;

public class ClusteringService(
    IStatusMessagingService statusMessagingService,
    IMessageService matchingMessagingService, 
    IClusteringRepository clusteringRepository,
    ILogger<ClusteringService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await matchingMessagingService.SubscribeAsync<MatchingScoreCandidatesFinishedMessage>(async (message) =>
            {
                await PreProcessAsync(stoppingToken);
            }, TMatchingQueue.MatchingScoreCandidatesFinished);

            await matchingMessagingService.SubscribeAsync<ClusteringPreProcessingFinishedMessage>(async (message) =>
            {
                await PostProcessAsync(stoppingToken);
            }, TMatchingQueue.ClusteringPreProcessingFinished);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred");
        }
    }

    private async Task PreProcessAsync(CancellationToken stoppingToken)
    {
        await clusteringRepository.ClusterPreProcessAsync();
        await matchingMessagingService.PublishAsync(new ClusteringPreProcessingStartedMessage());
    }

    private async Task PostProcessAsync(CancellationToken stoppingToken)
    {
        await statusMessagingService.StatusPublishAsync(new StatusUpdateMessage("Clustering (post-processing) started..."));

        await clusteringRepository.ClusterPostProcessAsync();
        await matchingMessagingService.PublishAsync(new ClusteringPostProcessingFinishedMessage());
    }

}
