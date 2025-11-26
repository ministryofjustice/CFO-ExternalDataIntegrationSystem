
using FileStorage;
using Messaging.Interfaces;
using Offloc.Cleaner.Cleaners;
using Serilog;

namespace Offloc.Cleaner.Services;

public abstract class CleaningStrategyBase
{
    protected IStagingMessagingService stagingService;
    protected IStatusMessagingService statusService;
    protected IFileLocations fileLocations;

    protected int[] redundantFieldIndexes;
    
    public CleaningStrategyBase(IStagingMessagingService stagingService, IStatusMessagingService statusService, 
        IFileLocations fileLocations, int[] redundantFieldIndexes)
    {
        this.stagingService = stagingService;
        this.statusService = statusService;
        this.redundantFieldIndexes = redundantFieldIndexes;
        this.fileLocations = fileLocations;
    }

    public virtual async Task ProcessFile(string file)
    {
        await Task.CompletedTask;

        FileCleaner fc = new FileCleaner(file, redundantFieldIndexes);
        // We are here. Lets try to clean the line 
        var clean = fc.Clean();
        // delete the original
        File.Delete(file);
        // move the clean one over the old name
        File.Move(clean, file);

        Log.Information($"File '{file}' has been cleaned, and is now ready for parsing");
    }
}