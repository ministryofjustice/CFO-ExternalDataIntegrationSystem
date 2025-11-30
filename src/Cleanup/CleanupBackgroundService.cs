using Cleanup.CleanupServices.LiveCleanup;
using Messaging.Interfaces;
using Messaging.Messages.MergingMessages.CleanupMessages;
using Messaging.Queues;
using Microsoft.Extensions.Hosting;
using Messaging.Messages.StagingMessages.Offloc;
using Messaging.Messages.StagingMessages.Delius;

namespace Cleanup;

public class CleanupBackgroundService(
    IMessageService messageService,
    DeliusCleanupService deliusCleanup,
    OfflocCleanupService offlocCleanup) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageService.SubscribeAsync<DeliusFilesCleanupMessage>(async message => 
        {
            deliusCleanup.Cleanup(message.FileName);
        }, TMergingQueue.DeliusFilesCleanupQueue);

        await messageService.SubscribeAsync<OfflocFilesCleanupMessage>(async message => 
        {
            offlocCleanup.Cleanup(message.FileName);
        }, TMergingQueue.OfflocFilesCleanupQueue);

        await messageService.SubscribeAsync<ClearTemporaryDeliusFiles>(async _ =>
        {
            deliusCleanup.ClearIllegalFiles();
        }, TStagingQueue.DeliusFilesClear);

        await messageService.SubscribeAsync<ClearHalfCleanedOfflocFiles>(async _ =>
        {
            offlocCleanup.ClearIllegalFiles();
        }, TStagingQueue.OfflocFilesClear);
    }
}
