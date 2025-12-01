
using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StatusMessages;
using Offloc.Parser.Processor;
using Offloc.Parser.Services.TrimmerContext;

namespace Offloc.Parser.Services;

public class ParsingStrategyBase(IMessageService messageService, IFileLocations fileLocations, FieldTrimmerContext trimmerContext)
{
    protected IMessageService messageService = messageService;
    protected IFileLocations fileLocations = fileLocations;

    protected async Task ParseFile(string fileName)
    {
        await messageService.PublishAsync(new StatusUpdateMessage($"Offloc parser started for file {fileName.Split('/').Last()}."));

        OfflocProcessor op = new OfflocProcessor(fileName, $"{fileLocations.offlocOutput}/{fileName.Split('/').Last()}", trimmerContext);
        await op.Process();
    }
}
