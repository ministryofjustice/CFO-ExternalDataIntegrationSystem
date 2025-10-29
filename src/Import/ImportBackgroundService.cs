using Messaging.Interfaces;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.ImportMessages;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using Microsoft.Extensions.Hosting;

namespace Import;

public class ImportBackgroundService : BackgroundService
{
    private readonly IStagingMessagingService messageService;
    private readonly IStatusMessagingService statusService;
    private readonly IDbMessagingService dbService;
    private readonly IImportMessagingService importMessagingService;

    //Uses semaphore like a lock/monitor. This is because locks/monitors
    //don't work in an asynchronous context.
    private static SemaphoreSlim offlocSem = new SemaphoreSlim(1, 1);
    private static SemaphoreSlim deliusSem = new SemaphoreSlim(1, 1);

    private bool deliusParserCompleted;
    private bool offlocParserCompleted;

    private bool deliusFileEmpty;
    private bool offlocFileEmpty;

    private bool[] parserStates => [deliusParserCompleted, offlocParserCompleted];
    private bool[] filesEmpty => [deliusFileEmpty, offlocFileEmpty];

    public ImportBackgroundService(IStagingMessagingService messageService, IStatusMessagingService statusService, IDbMessagingService dbService, IImportMessagingService importMessagingService)
    {
        this.dbService = dbService;
        this.messageService = messageService;
        this.statusService = statusService;
        this.importMessagingService=importMessagingService;
}

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(() =>
        {
            messageService.StagingSubscribe<OfflocParserFinishedMessage>(async (message) =>
            {
                await OnOfflocMessageReceived(message);
            }, TStagingQueue.OfflocImport);

            messageService.StagingSubscribe<DeliusParserFinishedMessage>(async (message) =>
            {
                await OnDeliusMessageReceived(message);
            }, TStagingQueue.DeliusImport);

        }, stoppingToken);
    }

    private async Task OnDeliusMessageReceived(DeliusParserFinishedMessage message)
    {
        await deliusSem.WaitAsync();

        if (message.emptyFile)
        {
            deliusFileEmpty = true;
            statusService.StatusPublish(new StatusUpdateMessage($"No Delius File to import (staging or merging)"));
        }
        else
        {
            statusService.StatusPublish(new StatusUpdateMessage($"Importing Delius File....."));
            var res = await dbService.DbTransientSubscribe<StageDeliusMessage, StageDeliusReturnMessage>(new StageDeliusMessage(message.fileName, message.filePath));
            var msg = await dbService.DbTransientSubscribe<MergeDeliusRunningPictureMessage, MergeDeliusReturnMessage>(new MergeDeliusRunningPictureMessage());
        }

        deliusSem.Release();

        deliusParserCompleted = true;
        await CheckStates();
    }

    private async Task OnOfflocMessageReceived(OfflocParserFinishedMessage message)
    {
        await offlocSem.WaitAsync();
           
        if (message.emptyFile)
        {
            offlocFileEmpty = true;
            statusService.StatusPublish(new StatusUpdateMessage($"No Offloc File to import (staging or merging)"));
        }
        else
        {
            statusService.StatusPublish(new StatusUpdateMessage($"Importing Offloc File....."));
            var res = await dbService.DbTransientSubscribe<StageOfflocMessage, StageOfflocReturnMessage>(new StageOfflocMessage(message.filePath));
            var msg = await dbService.DbTransientSubscribe<MergeOfflocRunningPictureMessage, MergeOfflocReturnMessage>
                    (new MergeOfflocRunningPictureMessage());
        }
        offlocSem.Release();
        offlocParserCompleted = true;

        await CheckStates();
    }

    //Purely for status updates.
    private async Task CheckStates()
    {
        await Task.CompletedTask;

        if (parserStates.All(b => b == true))
        {            
            deliusParserCompleted = false;
            offlocParserCompleted = false;

            if (filesEmpty.All(b => b == true))
            {
                statusService.StatusPublish(new StatusUpdateMessage($"no Files imported"));
            }
            else
            {
                //Clear staging Db after 10 minutes for testing purposes.
                //await Task.Delay(new TimeSpan(0, 10, 0));
                statusService.StatusPublish(new StatusUpdateMessage($"Files imported"));
                importMessagingService.ImportPublish(new ImportFinishedMessage());                     
            }
        }
    }
}
