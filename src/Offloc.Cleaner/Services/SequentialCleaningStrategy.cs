
using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StatusMessages;
using System.Globalization;

namespace Offloc.Cleaner.Services;

public class SequentialCleaningStrategy : CleaningStrategyBase, ICleaningStrategy
{    public SequentialCleaningStrategy(IStagingMessagingService stagingService, 
        IStatusMessagingService statusService, IFileLocations fileLocations, 
        RedundantFieldsWrapper redundantWrapper) 
        : base(stagingService, statusService, fileLocations, 
            redundantWrapper.redundantFieldIndexes){ }
    
    public async Task CleanFile(string file)
    {
        await statusService.StatusPublishAsync(new StatusUpdateMessage($"Cleaning file: {file}"));
        await base.ProcessFile(Path.Combine(fileLocations.offlocInput, file));
        await stagingService.StagingPublishAsync(new OfflocCleanerFinishedMessage([file], redundantFieldIndexes));
    }

}