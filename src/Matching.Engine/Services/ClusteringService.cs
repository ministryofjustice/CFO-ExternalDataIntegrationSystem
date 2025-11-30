using Messaging.Interfaces;
using Messaging.Messages.MatchingMessages;
using Messaging.Messages.MatchingMessages.Clustering;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Matching.Engine.Services;

public class ClusteringService(
    IMessageService messageService, 
    IClusteringRepository clusteringRepository,
    ILogger<ClusteringService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await messageService.SubscribeAsync<MatchingScoreCandidatesFinishedMessage>(async (message) =>
            {
                await PreProcessAsync(stoppingToken);
            }, TMatchingQueue.MatchingScoreCandidatesFinished);

            await messageService.SubscribeAsync<ClusteringPreProcessingFinishedMessage>(async (message) =>
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
        await messageService.PublishAsync(new ClusteringPreProcessingStartedMessage());
    }

    private async Task PostProcessAsync(CancellationToken stoppingToken)
    {
        await messageService.PublishAsync(new StatusUpdateMessage("Clustering (post-processing) started..."));

        await clusteringRepository.ClusterPostProcessAsync();
        await messageService.PublishAsync(new ClusteringPostProcessingFinishedMessage());
    }

}
