
using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StatusMessages;

namespace Offloc.Cleaner.Services;

public class ParallelCleaningStrategy : CleaningStrategyBase, ICleaningStrategy
{
    public ParallelCleaningStrategy(IStagingMessagingService stagingService, 
        IStatusMessagingService statusService, IFileLocations fileLocations, 
        int[] redundantFields) 
        : base(stagingService, statusService, fileLocations, redundantFields) { }

    public async Task CleanFiles(string[] files)
    {
        await Task.CompletedTask;

        List<Task> tasks = new List<Task>();

        Parallel.For(0, files.Length, (i, f) => {
            statusService.StatusPublish(new StatusUpdateMessage($"Processing file: {files[i]}"));
            Task t = CleanFile($"{fileLocations.offlocInput}/{files[i]}");
            tasks.Add(t);
        });

        Task.WaitAll(tasks.ToArray()); //Waits for all cleaning to finish so parser can run in parallel easily.

        if (files.Length > 0)
        {
            stagingService.StagingPublish(new OfflocCleanerFinishedMessage(files, redundantFieldIndexes));
        }
    }
}
