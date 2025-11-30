using Messaging.Interfaces;
using Microsoft.Extensions.Hosting;
using Messaging.Queues;
using Messaging.Messages.ImportMessages;
using Messaging.Messages.BlockingFinishedMessages;
using Messaging.Messages.StatusMessages;

namespace Blocking;

public class BlockingBackgroundService(
    IMessageService messageService,
    DatabaseInsert database) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageService.SubscribeAsync<ImportFinishedMessage>(async (message) =>
        {
            await messageService.PublishAsync(new StatusUpdateMessage("Blocking candidates..."));
            await CallBlocking();
        }, TImportQueue.ImportFinished);
    }

    private async Task CallBlocking()
    {
        await database.InsertCandidates();
        await messageService.PublishAsync(new BlockingFinishedMessage());        
    }
}
