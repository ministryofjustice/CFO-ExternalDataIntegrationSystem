using Delius.Parser.Services;
using Messaging.Interfaces;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using Microsoft.Extensions.Hosting;

namespace Delius.Parser;

public class DeliusParserBackgroundService(
    IMessageService messageService,
    IParsingStrategy parseService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageService.SubscribeAsync<DeliusDownloadFinishedMessage>(async (message) => 
        { 
            await ParseFileAsync(message); 
        }, TStagingQueue.DeliusParser);
    }

    private async Task ParseFileAsync(DeliusDownloadFinishedMessage message)
    {
        var file = message.FileName;

        if (await HasAlreadyBeenProcessedAsync(file))
        {
            await messageService.PublishAsync(new StatusUpdateMessage($"File {file} has already been processed"));
            await messageService.PublishAsync(new DeliusParserFinishedMessage("File already processed", "No Path", emptyFile: true));
        }
        else
        {
            await BeginProcessing(file, message.FileId);
        }
    }
    
    private async Task BeginProcessing(string fileName, string fileId)
    {
        var request = new DeliusFileProcessingStarted(fileName, fileId);
        await messageService.SendDbRequestAndWaitForResponseAsync<DeliusFileProcessingStarted, ResultDeliusFileProcessingStarted>(request);
        await parseService.ParseFileAsync(fileName);
    }

    private async Task<bool> HasAlreadyBeenProcessedAsync(string file)
    {
        var res = await messageService.SendDbRequestAndWaitForResponseAsync<GetDeliusFilesMessage, DeliusFilesReturnMessage>(new GetDeliusFilesMessage());
        return res.FileNames.Contains(file);
    }
}