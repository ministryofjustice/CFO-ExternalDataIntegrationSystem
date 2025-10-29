
using Messaging.Interfaces;
using Messaging.Messages.StagingMessages;
using Messaging.Queues;
using Microsoft.Extensions.Hosting;
using Offloc.Parser.Services;

namespace Offloc.Parser;

public class OfflocParserBackgroundService : BackgroundService
{
    private readonly IStagingMessagingService messageService;
    private readonly IParsingStrategy parsingService;
    private readonly IStatusMessagingService statusService;

    public OfflocParserBackgroundService(IStagingMessagingService messageService, 
        IParsingStrategy parsingService, IStatusMessagingService statusService)
    {
        this.messageService = messageService;
        this.parsingService = parsingService;
        this.statusService = statusService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.CompletedTask;

        messageService.StagingSubscribe<OfflocCleanerFinishedMessage>(async(message) =>
        {
            //For parallel processing, the file id is insignificant.
            //For sequential processing, filesToParse is always of length 1.
            await parsingService.ParseFiles(message.filesToParse);
        }, TStagingQueue.OfflocParser);
    }
}
