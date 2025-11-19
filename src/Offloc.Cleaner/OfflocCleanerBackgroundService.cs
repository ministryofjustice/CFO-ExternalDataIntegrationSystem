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
    IStagingMessagingService stagingService,
    IDbMessagingService dbService,
    ICleaningStrategy cleaningService,
    IStatusMessagingService statusService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(() =>
        {
            stagingService.StagingSubscribe<OfflocDownloadFinished>(async (message) => await ParseFileAsync(message), TStagingQueue.OfflocCleaner);
        }, stoppingToken);
    }

    private async Task ParseFileAsync(OfflocDownloadFinished message)
    {
        string file = message.fileName;

        if (await HasAlreadyBeenProcessedAsync(file))
        {
            statusService.StatusPublish(new StatusUpdateMessage($"File {file} has already been processed"));
            stagingService.StagingPublish(new OfflocParserFinishedMessage("File already processed", true));
        }
        else
        {
            var request = new OfflocFileProcessingStarted(message.fileName, message.FileId, message.ArchiveFileName);
            await dbService.SendDbRequestAndWaitForResponse<OfflocFileProcessingStarted, ResultOfflocFileProcessingStarted>(request);
            await cleaningService.CleanFile(file);
        }
    }
    private async Task<bool> HasAlreadyBeenProcessedAsync(string file)
    {
        var res = await dbService.SendDbRequestAndWaitForResponse<GetOfflocFilesMessage, OfflocFilesReturnMessage>(new GetOfflocFilesMessage());
        return res.offlocFiles.Contains(file);
    }
}
