using System.Globalization;
using EnvironmentSetup;
using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using Microsoft.Extensions.Hosting;
using Offloc.Cleaner.Services;

namespace Offloc.Cleaner;

public class OfflocCleanerBackgroundService(
    IMessageService messageService,
    IDbMessagingService dbService,
    ICleaningStrategy cleaningService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageService.SubscribeAsync<OfflocDownloadFinished>(async (message) => 
        {
            await ParseFileAsync(message);
        }, TStagingQueue.OfflocCleaner);
    }

    private async Task ParseFileAsync(OfflocDownloadFinished message)
    {
        string file = message.FileName;

        if (await HasAlreadyBeenProcessedAsync(file))
        {
            await messageService.PublishAsync(new StatusUpdateMessage($"File {file} has already been processed"));
            await messageService.PublishAsync(new OfflocParserFinishedMessage("File already processed", emptyFile: true));
        }
        else
        {
            var request = new OfflocFileProcessingStarted(message.FileName, message.FileId, message.ArchiveFileName);
            await dbService.SendDbRequestAndWaitForResponseAsync<OfflocFileProcessingStarted, ResultOfflocFileProcessingStarted>(request);
            await cleaningService.CleanFile(file);
        }
    }
    private async Task<bool> HasAlreadyBeenProcessedAsync(string file)
    {
        var res = await dbService.SendDbRequestAndWaitForResponseAsync<GetOfflocFilesMessage, OfflocFilesReturnMessage>(new GetOfflocFilesMessage());
        return res.OfflocFiles.Contains(file);
    }
}
