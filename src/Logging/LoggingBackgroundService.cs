using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using Microsoft.Extensions.Logging;

namespace Logging;

public class LoggingBackgroundService(
    ILogger<LoggingBackgroundService> logger, 
    IStatusMessagingService statusService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
        await statusService.StatusSubscribeAsync<StatusUpdateMessage>(Log, TStatusQueue.StatusUpdate);
        await statusService.StatusSubscribeAsync<StagingFinishedMessage>(Log, TStatusQueue.StagingFinished);

        await statusService.StatusPublishAsync(new StatusUpdateMessage("Logger configured."));
	}

    private void Log(StatusUpdateMessage statusUpdate)
    {
        logger.LogInformation($"{statusUpdate.Caller}: {statusUpdate.Message}");
    }
}
