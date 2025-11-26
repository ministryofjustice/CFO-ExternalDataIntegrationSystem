using Cleanup.CleanupServices.LiveCleanup;

using Messaging.Interfaces;
using Messaging.Messages.MergingMessages.CleanupMessages;
using Messaging.Queues;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using Messaging.Messages.StagingMessages.Offloc;
using Messaging.Messages.StagingMessages.Delius;

namespace Cleanup;

public class CleanupBackgroundService : BackgroundService
{
    private readonly IStagingMessagingService stagingMessagingService;
    private readonly IMergingMessagingService mergingMessagingService;
    private readonly IDbMessagingService dbMessagingService;

    private DeliusCleanupService deliusCleanup;
    private OfflocCleanupService offlocCleanup;

    private CultureInfo cultureInfo = new CultureInfo("en-GB");

    public CleanupBackgroundService(IMergingMessagingService mergingService, 
        IDbMessagingService dbMessagingService, IStagingMessagingService stagingService,
        OfflocCleanupService offlocCleanup, DeliusCleanupService deliusCleanup) 
    {
        this.stagingMessagingService = stagingService;
        this.mergingMessagingService = mergingService;
        this.deliusCleanup = deliusCleanup;
        this.offlocCleanup = offlocCleanup;
        this.dbMessagingService = dbMessagingService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.CompletedTask;

        mergingMessagingService.MergingSubscribeAsync<DeliusFilesCleanupMessage>(async(message) => 
            await CleanDelius(message), TMergingQueue.DeliusFilesCleanupQueue);
        mergingMessagingService.MergingSubscribeAsync<OfflocFilesCleanupMessage>(async(message) => 
            await CleanOffloc(message), TMergingQueue.OfflocFilesCleanupQueue);

        stagingMessagingService.StagingSubscribeAsync<ClearTemporaryDeliusFiles>(async (message) =>
        {
            await Task.CompletedTask;
            deliusCleanup.ClearIllegalFiles();
        }, TStagingQueue.DeliusFilesClear);

        stagingMessagingService.StagingSubscribeAsync<ClearHalfCleanedOfflocFiles>(async (message) =>
        {
            await Task.CompletedTask;
            offlocCleanup.ClearIllegalFiles();
        }, TStagingQueue.OfflocFilesClear);
    }

    private async Task CleanDelius(DeliusFilesCleanupMessage message)
    {
        await Task.CompletedTask;

        deliusCleanup.Cleanup(message.fileName);
    }

    private async Task CleanOffloc(OfflocFilesCleanupMessage message)
    {
        await Task.CompletedTask;

        offlocCleanup.Cleanup(message.fileName);
    }
}
