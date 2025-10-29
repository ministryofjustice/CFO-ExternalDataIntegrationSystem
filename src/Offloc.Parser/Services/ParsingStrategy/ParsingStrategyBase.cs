
using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StatusMessages;
using Offloc.Parser.Processor;
using Offloc.Parser.Services.TrimmerContext;

namespace Offloc.Parser.Services;

public class ParsingStrategyBase
{
    protected IStagingMessagingService stagingService;
    protected IStatusMessagingService statusService;
    protected IFileLocations fileLocations;

    private FieldTrimmerContext trimmerContext;

    public ParsingStrategyBase(IStagingMessagingService stagingService, IStatusMessagingService statusService,
        IFileLocations fileLocations, FieldTrimmerContext trimmerContext)
    {
        this.stagingService = stagingService;
        this.statusService = statusService;
        this.trimmerContext = trimmerContext;
        this.fileLocations = fileLocations;
    }

    protected async Task ParseFile(string fileName)
    {
        statusService.StatusPublish(new StatusUpdateMessage($"Offloc parser started for file {fileName.Split('/').Last()}."));

        OfflocProcessor op = new OfflocProcessor(fileName, $"{fileLocations.offlocOutput}/{fileName.Split('/').Last()}", trimmerContext);
        await op.Process();
    }
}
