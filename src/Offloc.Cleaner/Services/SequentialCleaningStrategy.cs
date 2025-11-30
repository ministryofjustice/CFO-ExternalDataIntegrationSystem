
using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StatusMessages;

namespace Offloc.Cleaner.Services;

public class SequentialCleaningStrategy(
    IMessageService messageService, 
    IFileLocations fileLocations,
    RedundantFieldsWrapper redundantWrapper) : CleaningStrategyBase(redundantWrapper.redundantFieldIndexes), ICleaningStrategy
{
    public async Task CleanFile(string file)
    {
        await messageService.PublishAsync(new StatusUpdateMessage($"Cleaning file: {file}"));
        ProcessFile(Path.Combine(fileLocations.offlocInput, file));
        await messageService.PublishAsync(new OfflocCleanerFinishedMessage([file], redundantWrapper.redundantFieldIndexes));
    }

}