using Delius.Parser.Core;
using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StatusMessages;

namespace Delius.Parser.Services;

public abstract class ParsingStrategyBase 
{
    protected readonly IFileLocations fileLocations;
    protected readonly IFileProcessor fileProcessor;
    protected readonly IStatusMessagingService statusService;
    protected readonly IStagingMessagingService stagingService;

    public ParsingStrategyBase(IFileProcessor fp, IStatusMessagingService statusService,
        IStagingMessagingService stagingService, IFileLocations fileLocations)
    {
        this.statusService = statusService;
        this.stagingService = stagingService;
        fileProcessor = fp;
        this.fileLocations = fileLocations;
    }

    public async Task ProcessFile(string file)
    {            
        await statusService.StatusPublishAsync(new StatusUpdateMessage($"Delius parser started on file {file}."));

        await fileProcessor.Process(fileLocations.deliusInput + '/' + file, $"{fileLocations.deliusOutput}/{file.Split('.').First()}");

        await stagingService.StagingPublishAsync(new DeliusParserFinishedMessage(file, fileLocations.deliusInput + '/' + file.Split('.').First(), false));
    }
}
