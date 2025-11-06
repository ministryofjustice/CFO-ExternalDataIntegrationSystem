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
    IParsingStrategy parseService,
    IFileLocations fileLocations,
    SystemFileSource systemFileSource) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(() => 
        {
            messageService.StagingSubscribe<DeliusDownloadFinishedMessage>(async (message) => await ParseFilesAsync(), TStagingQueue.DeliusParser);
        }, stoppingToken);
    }

    private async Task ParseFilesAsync()
    {
        string[] files = await GetFilesAsync();

        if (files.Length == 0)
        {
            statusService.StatusPublish(new StatusUpdateMessage($"No files found in {fileLocations.deliusInput}"));
            messageService.StagingPublish(new DeliusParserFinishedMessage("No Files", "No Path", true));            
        }
        else
        {
            await parseService.ParseFiles(files);
        }
    }

    //Gets a list of files in the target directory.
    private async Task<string[]> GetFilesAsync()
    {
        var filesWithPath = await systemFileSource.ListDeliusFilesAsync(fileLocations.deliusInput);

        var fileNames = filesWithPath.Select(file => Path.GetFileName(file)!);
        
        var processedFiles = await GetAlreadyProcessedFiles();

        var unprocessedFiles = fileNames.Except(processedFiles);

        // cfoextract_0001_...dat -> will return files in (unique) id ascending order (0001, 0002, 0003, etc)
        // Newer files appear later in the collection
        return unprocessedFiles
            .OrderBy(fileName => fileName)
            .ToArray();
    }

    private async Task<string[]> GetAlreadyProcessedFiles()
    {
        var res = await dbService.DbTransientSubscribe<GetDeliusFilesMessage, DeliusFilesReturnMessage>(new GetDeliusFilesMessage());

        return res.fileNames;
    }
}