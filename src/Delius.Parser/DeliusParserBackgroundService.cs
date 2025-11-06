using Delius.Parser.Services;
using EnvironmentSetup;
using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using Microsoft.Extensions.Hosting;

namespace Delius.Parser;

public class DeliusParserBackgroundService(
    IStagingMessagingService messageService,
    IDbMessagingService dbService,
    IStatusMessagingService statusService,
    IParsingStrategy parseService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(() => 
        {
            messageService.StagingSubscribe<DeliusDownloadFinishedMessage>(async (message) => await ParseFileAsync(message.File), TStagingQueue.DeliusParser);
        }, stoppingToken);
    }

    private async Task ParseFileAsync(string file)
    {
        if (await HasAlreadyBeenProcessedAsync(file))
        {
            statusService.StatusPublish(new StatusUpdateMessage($"File {file} has already been processed"));
            messageService.StagingPublish(new DeliusParserFinishedMessage("File already processed", "No Path", true));    
        }
        else
        {
            await parseService.ParseFileAsync(file);
        }
    }

    private async Task<bool> HasAlreadyBeenProcessedAsync(string file)
    {
        var res = await dbService.DbTransientSubscribe<GetDeliusFilesMessage, DeliusFilesReturnMessage>(new GetDeliusFilesMessage());
        return res.fileNames.Contains(file);
    }
}