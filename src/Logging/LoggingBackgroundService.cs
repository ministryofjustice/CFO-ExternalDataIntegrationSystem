using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using Microsoft.Extensions.Logging;

namespace Logging;

public class LoggingBackgroundService(
    ILogger<LoggingBackgroundService> logger, 
    IMessageService messageService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
        await messageService.SubscribeAsync<StatusUpdateMessage>(Log, TStatusQueue.StatusUpdate);
        await messageService.SubscribeAsync<StagingFinishedMessage>(Log, TStatusQueue.StagingFinished);
        await messageService.PublishAsync(new StatusUpdateMessage("Logger configured."));
	}

    private void Log(StatusUpdateMessage statusUpdate)
    {
        logger.LogInformation($"{statusUpdate.Caller}: {statusUpdate.Message}");
    }
}
