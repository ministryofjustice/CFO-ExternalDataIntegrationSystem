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

public class OfflocCleanerBackgroundService : BackgroundService
{    
    private readonly IStagingMessagingService stagingService;
    private readonly IDbMessagingService dbService;
    private readonly IStatusMessagingService statusService;
    private readonly IFileLocations fileLocations;
    private readonly ICleaningStrategy cleaningService;

    public OfflocCleanerBackgroundService(IStagingMessagingService stagingService, 
        IDbMessagingService dbService, ICleaningStrategy cleaningService, 
        IStatusMessagingService statusService, IFileLocations fileLocations)
    {
        this.stagingService = stagingService;
        this.dbService = dbService;
        this.statusService = statusService;
        this.cleaningService = cleaningService;
        this.fileLocations = fileLocations;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //stagingService.StagingSubscribe<OfflocKickoffMessage>(async (message) => await ParseFile(), TStagingQueue.OfflocCleaner);
        await Task.Run(() =>
        {
            stagingService.StagingSubscribe<OfflocDownloadFinished>(async (message) => await ParseFilesAsync(), TStagingQueue.OfflocCleaner);
        }, stoppingToken);
    }

    private async Task ParseFilesAsync()
    {
        string[] files = await GetFilesAsync();

        if (files == null || files.Length == 0)
        {
            statusService.StatusPublish(new StatusUpdateMessage($"No files found in '{fileLocations.offlocInput}'"));
            stagingService.StagingPublish(new OfflocParserFinishedMessage("No Path", true));
        }
        else
        {
            await cleaningService.CleanFiles(files);
        }
    }

    //Returns files that fit validation checks (ie. not already been processed).
    private async Task<string[]> GetFilesAsync()
    {
        string[] files = Directory.GetFiles(fileLocations.offlocInput, "*.dat").Where(f => !f.Contains("LineFeeds")).ToArray();

        for (int i = 0; i < files.Length; i++)
        {
            files[i] = files[i][(fileLocations.offlocInput.Length + 1)..];
        }

        var res = await GetAlreadyProcessedFilesAsync();

        return files.Where(f => !res.Contains(f)).ToArray();
    }

    private async Task<string[]> GetAlreadyProcessedFilesAsync()
    {
        var res = await dbService.DbTransientSubscribe<GetOfflocFilesMessage, OfflocFilesReturnMessage>(new GetOfflocFilesMessage());
        return res.offlocFiles;
    }
}
