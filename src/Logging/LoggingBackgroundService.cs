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
        await Task.Run(() =>
        {
            statusService.StatusSubscribe<StatusUpdateMessage>(Log, TStatusQueue.StatusUpdate);
            statusService.StatusSubscribe<StagingFinishedMessage>(Log, TStatusQueue.StagingFinished);
        }, stoppingToken);

        statusService.StatusPublish(new StatusUpdateMessage("Logger configured."));
	}

    private void Log(StatusUpdateMessage statusUpdate)
    {
        logger.LogInformation($"{statusUpdate.Caller}: {statusUpdate.Message}");
    }
}
