using Delius.Parser.Services;
using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using Microsoft.Extensions.Hosting;
using System.Globalization;

namespace Delius.Parser;

public class DeliusParserBackgroundService : BackgroundService
{
    private readonly IStagingMessagingService messageService;
    private readonly IDbMessagingService dbService;
    private readonly IStatusMessagingService statusService;
    private readonly IParsingStrategy parseService;
    private readonly IFileLocations fileLocations;

    private CultureInfo cultureInfo = new CultureInfo("en-GB");
    
    public DeliusParserBackgroundService(IStagingMessagingService messageService, 
        IDbMessagingService dbService, IStatusMessagingService statusService, 
        IParsingStrategy parseStrategy, IFileLocations fileLocations)
    {
        this.messageService = messageService;
        this.statusService = statusService;
        this.dbService = dbService;
        parseService = parseStrategy;
        this.fileLocations = fileLocations;
    }

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
        var files = Directory.GetFiles(fileLocations.deliusInput, "*.txt");

        for (int i = 0; i<files.Length; i++)
        {
            FileInfo f = new FileInfo(files[i]); //TEST- Use to order sequential processing of files.
            files[i] = files[i][(fileLocations.deliusInput.Length+1)..];
        }
        
        var res = await GetAlreadyProcessedFiles();

        return files
            .Where(f => !res.Contains(f))
            .OrderBy(f => DateOnly.Parse($"{f[27..29]}/{f[25..27]}/{f[21..25]}", cultureInfo))
            .ToArray();
    }

    private async Task<string[]> GetAlreadyProcessedFiles()
    {
        var res = await dbService.DbTransientSubscribe<GetDeliusFilesMessage, DeliusFilesReturnMessage>(new GetDeliusFilesMessage());

        return res.fileNames;
    }
}