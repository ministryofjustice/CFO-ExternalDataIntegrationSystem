using Messaging.Interfaces;
using Messaging.Messages.StagingMessages;
using Messaging.Queues;
using Microsoft.Extensions.Hosting;
using Offloc.Parser.Services;

namespace Offloc.Parser;

public class OfflocParserBackgroundService(
    IMessageService messageService,
    IParsingStrategy parsingService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageService.SubscribeAsync<OfflocCleanerFinishedMessage>(async message =>
        {
            await parsingService.ParseFiles(message.filesToParse);
        }, TStagingQueue.OfflocParser);
    }
}
