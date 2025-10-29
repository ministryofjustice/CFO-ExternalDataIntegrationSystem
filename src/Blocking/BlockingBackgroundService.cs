using Messaging.Interfaces;
using Microsoft.Extensions.Hosting;
using Messaging.Queues;
using Messaging.Messages.ImportMessages;
using Messaging.Messages.BlockingFinishedMessages;
using Messaging.Messages.StatusMessages;

namespace Blocking;

public class BlockingBackgroundService : BackgroundService
{
    private readonly IStatusMessagingService statusMessagingService;
    private readonly IImportMessagingService importMessageService;
    private readonly IBlockingMessagingService blockingMessageService;
    private readonly DatabaseInsert matchingDbInsert;

    public BlockingBackgroundService(IStatusMessagingService statusMessagingService, IImportMessagingService importMessageService,
        DatabaseInsert matchingDbInsert, IBlockingMessagingService blockingMessageService)
    {
        this.statusMessagingService = statusMessagingService;
        this.matchingDbInsert = matchingDbInsert;
        this.importMessageService = importMessageService;
        this.blockingMessageService = blockingMessageService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(() =>
        {
            importMessageService.ImportSubscribe<ImportFinishedMessage>(async (message) =>
            {
                statusMessagingService.StatusPublish(new StatusUpdateMessage("Blocking candidates..."));
                await CallBlocking();
            }, TImportQueue.ImportFinished);
        }, stoppingToken);
    }

    private async Task CallBlocking()
    {
        await matchingDbInsert.InsertCandidates();
        blockingMessageService.BlockingPublish(new BlockingFinishedMessage());        
    }
}
