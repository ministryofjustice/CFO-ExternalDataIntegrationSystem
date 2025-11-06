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
    IStatusMessagingService statusService,
    IFileLocations fileLocations,
    SystemFileSource fileSource) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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
        var filesWithPath = await fileSource.ListOfflocFilesAsync(fileLocations.offlocInput);

        var fileNames = filesWithPath.Select(file => Path.GetFileName(file)!);

        var processedFiles = await GetAlreadyProcessedFilesAsync();

        var unprocessedFiles = fileNames.Except(processedFiles);
    
        // Newer files appear later in the collection
        return unprocessedFiles
            .OrderBy(Datestamp)
            .ThenBy(Index)
            .ToArray();
    }

    private int Index(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        var parts = name.Split('_');
        return int.Parse(parts[4]);
    }

    private DateOnly Datestamp(string fileName)
    {
        // C_NOMIS_OFFENDER_ddMMyyyy_01.dat
        var parts = fileName.Split('_');

        // 0. C
        // 1. NOMIS
        // 2. OFFENDER
        // 3. ddMMyyyy
        // 4. 01 (or 02, 03, 03, etc,.)
        var datePart = parts[3];

        return DateOnly.ParseExact(datePart, "ddMMyyyy", CultureInfo.InvariantCulture);
    }

    private async Task<string[]> GetAlreadyProcessedFilesAsync()
    {
        var res = await dbService.DbTransientSubscribe<GetOfflocFilesMessage, OfflocFilesReturnMessage>(new GetOfflocFilesMessage());
        return res.offlocFiles;
    }
}
