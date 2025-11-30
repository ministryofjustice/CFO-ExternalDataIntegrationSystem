using Messaging.Interfaces;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.ImportMessages;
using Messaging.Messages.StagingMessages;
using Messaging.Queues;
using Microsoft.Extensions.Hosting;

namespace Import;

public class ImportBackgroundService(IMessageService messageService) : BackgroundService
{
    private static readonly SemaphoreSlim offlocSem = new(1, 1);
    private static readonly SemaphoreSlim deliusSem = new(1, 1);

    private bool deliusParserCompleted;
    private bool offlocParserCompleted;
    private bool deliusFileEmpty;
    private bool offlocFileEmpty;

    private bool[] ParserStates => [deliusParserCompleted, offlocParserCompleted];
    private bool[] FilesEmpty => [deliusFileEmpty, offlocFileEmpty];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageService.SubscribeAsync<OfflocParserFinishedMessage>(
            async message => await OnOfflocMessageReceived(message), 
            TStagingQueue.OfflocImport);

        await messageService.SubscribeAsync<DeliusParserFinishedMessage>(
            async message => await OnDeliusMessageReceived(message), 
            TStagingQueue.DeliusImport);
    }

    private async Task OnDeliusMessageReceived(DeliusParserFinishedMessage message)
    {
        await deliusSem.WaitAsync();

        if (message.EmptyFile)
        {
            deliusFileEmpty = true;
        }
        else
        {
            await messageService.SendDbRequestAndWaitForResponseAsync<StageDeliusMessage, StageDeliusReturnMessage>(
                new StageDeliusMessage(message.FileName, message.FilePath));
            await messageService.SendDbRequestAndWaitForResponseAsync<MergeDeliusRunningPictureMessage, MergeDeliusReturnMessage>(
                new MergeDeliusRunningPictureMessage(message.FileName));
        }

        deliusSem.Release();
        deliusParserCompleted = true;
        await CheckStates();
    }

    private async Task OnOfflocMessageReceived(OfflocParserFinishedMessage message)
    {
        await offlocSem.WaitAsync();
           
        if (message.EmptyFile)
        {
            offlocFileEmpty = true;
        }
        else
        {
            await messageService.SendDbRequestAndWaitForResponseAsync<StageOfflocMessage, StageOfflocReturnMessage>(
                new StageOfflocMessage(message.FilePath));
            await messageService.SendDbRequestAndWaitForResponseAsync<MergeOfflocRunningPictureMessage, MergeOfflocReturnMessage>(
                new MergeOfflocRunningPictureMessage(Path.GetFileName(message.FilePath)));
        }

        offlocSem.Release();
        offlocParserCompleted = true;
        await CheckStates();
    }

    private async Task CheckStates()
    {
        if (ParserStates.All(b => b))
        {            
            deliusParserCompleted = false;
            offlocParserCompleted = false;

            if (!FilesEmpty.All(b => b))
            {
                await messageService.PublishAsync(new ImportFinishedMessage());                     
            }
        }
    }
}
