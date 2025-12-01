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
    protected readonly IMessageService messageService;

    public ParsingStrategyBase(IFileProcessor fp, IMessageService messageService, IFileLocations fileLocations)
    {
        this.messageService = messageService;
        fileProcessor = fp;
        this.fileLocations = fileLocations;
    }

    public async Task ProcessFileAsync(string file)
    {            
        await messageService.PublishAsync(new StatusUpdateMessage($"Delius parser started on file {file}."));

        await fileProcessor.Process(fileLocations.deliusInput + '/' + file, $"{fileLocations.deliusOutput}/{file.Split('.').First()}");

        await messageService.PublishAsync(new DeliusParserFinishedMessage(file, fileLocations.deliusInput + '/' + file.Split('.').First(), false));
    }
}
