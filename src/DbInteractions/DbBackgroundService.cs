
using DbInteractions.Services;
using Messaging.Interfaces;
using Messaging.Messages.DbMessages;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.MergingMessages.CleanupMessages;
using Messaging.Queues;
using Microsoft.Extensions.Hosting;

namespace DbInteractions;

//This class links the messaging service and dbInteractionService- the dbinteraction service does all of the work. 
public class DbBackgroundService : BackgroundService
{
    private readonly IDbInteractionService dbInteractionService;
    private readonly IDbMessagingService dbMessagingService;
    private readonly IMergingMessagingService mergingService;

    public DbBackgroundService(IDbInteractionService dbInteractionService, IDbMessagingService dbMessagingService,
        IMergingMessagingService mergingService)
    {
        this.dbInteractionService = dbInteractionService;
        this.dbMessagingService = dbMessagingService;
        this.mergingService = mergingService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.CompletedTask;

        dbMessagingService.SubscribeToDbRequest<GetDeliusFilesMessage>(async (message) =>
        {
            string[] results = await dbInteractionService.GetProcessedDeliusFileNames();
            DeliusFilesReturnMessage msg = new DeliusFilesReturnMessage(results);
            dbMessagingService.DbPublishResponse(msg);
        }, TDbQueue.GetProcessedDeliusFiles);

        dbMessagingService.SubscribeToDbRequest<GetOfflocFilesMessage>(async (message) =>
        {
            string[] results = await dbInteractionService.GetProcessedOfflocFileNames();
            dbMessagingService.DbPublishResponse(new OfflocFilesReturnMessage(results));
        }, TDbQueue.GetProcessedOfflocFiles);

        dbMessagingService.SubscribeToDbRequest<StageDeliusMessage>(async (message) =>
        {
            await dbInteractionService.StageDelius(message.fileName, message.filePath);
            dbMessagingService.DbPublishResponse(new StageDeliusReturnMessage());
            mergingService.MergingPublish(new DeliusFilesCleanupMessage(message.fileName));
        }, TDbQueue.StageDelius);

        dbMessagingService.SubscribeToDbRequest<MergeDeliusRunningPictureMessage>(async (message) =>
        {
            //await dbInteractionService.StandardiseDeliusStaging();
            await dbInteractionService.MergeDeliusPicture(message.fileName);
            await dbInteractionService.ClearDeliusStaging();

            dbMessagingService.DbPublishResponse(new MergeDeliusReturnMessage());
        }, TDbQueue.MergeDelius);

        dbMessagingService.SubscribeToDbRequest<ClearDeliusStaging>(async (message) =>
        {
            await dbInteractionService.ClearDeliusStaging();
            dbMessagingService.DbPublishResponse(new ResultClearDeliusStaging());
        }, TDbQueue.ClearDeliusStaging);

        dbMessagingService.SubscribeToDbRequest<StageOfflocMessage>(async (message) =>
        {
            await dbInteractionService.StageOffloc(message.fileName);
            dbMessagingService.DbPublishResponse(new StageOfflocReturnMessage());

            mergingService.MergingPublish(new OfflocFilesCleanupMessage(message.fileName));
        }, TDbQueue.StageOffloc);

        dbMessagingService.SubscribeToDbRequest<MergeOfflocRunningPictureMessage>(async (message) =>
        {
            await dbInteractionService.MergeOfflocPicture(message.fileName);
            await dbInteractionService.ClearOfflocStaging();

            dbMessagingService.DbPublishResponse(new MergeOfflocReturnMessage());

        }, TDbQueue.MergeOffloc);

        dbMessagingService.SubscribeToDbRequest<ClearOfflocStaging>(async (message) =>
        {
            await dbInteractionService.ClearOfflocStaging();
            dbMessagingService.DbPublishResponse(new ResultClearOfflocStaging());
        }, TDbQueue.ClearOfflocStaging);

        dbMessagingService.SubscribeToDbRequest<DeliusGetLastFullMessage>(async (message) =>
        {
            int res = await dbInteractionService.DeliusGetFileIdLastFull();
            dbMessagingService.DbPublishResponse(new ResultDeliusGetLastFullMessage(res));
        }, TDbQueue.DeliusGetLastFullId);

        dbMessagingService.SubscribeToDbRequest<OfflocFileProcessingStarted>(async (message) =>
        {
            await dbInteractionService.CreateOfflocProcessedFileEntry(message.fileName, message.fileId, message.archiveName);
            dbMessagingService.DbPublishResponse(new ResultOfflocFileProcessingStarted());
        }, TDbQueue.OfflocFileProcessingStarted);

        dbMessagingService.SubscribeToDbRequest<DeliusFileProcessingStarted>(async (message) =>
        {
            await dbInteractionService.CreateDeliusProcessedFileEntry(message.fileName, message.fileId);
            dbMessagingService.DbPublishResponse(new ResultDeliusFileProcessingStarted());
        }, TDbQueue.DeliusFileProcessingStarted);

        dbMessagingService.SubscribeToDbRequest<AssociateOfflocFileWithArchiveMessage>(async (message) =>
        {
            await dbInteractionService.AssociateOfflocFileWithArchive(message.fileName, message.archiveName);
            dbMessagingService.DbPublishResponse(new ResultAssociateOfflocFileWithArchiveMessage());
        }, TDbQueue.AssociateOfflocFileWithArchive);

        dbMessagingService.SubscribeToDbRequest<IsDeliusReadyForProcessingMessage>(async (message) =>
        {
            bool result = await dbInteractionService.IsDeliusReadyForProcessing();
            dbMessagingService.DbPublishResponse(new IsDeliusReadyForProcessingReturnMessage(result));
        }, TDbQueue.IsDeliusReadyForProcessing);
        
        
        dbMessagingService.SubscribeToDbRequest<IsOfflocReadyForProcessingMessage>(async (message) =>
        {
            bool result = await dbInteractionService.IsOfflocReadyForProcessing();
            dbMessagingService.DbPublishResponse(new IsOfflocReadyForProcessingReturnMessage(result));
        }, TDbQueue.IsOfflocReadyForProcessing);

        dbMessagingService.SubscribeToDbRequest<GetLastProcessedOfflocFile>(async (message) =>
        {
            string? result = await dbInteractionService.GetLastProcessedOfflocFileName();
            dbMessagingService.DbPublishResponse(new ResultGetLastProcessedOfflocFileMessage(result));
        }, TDbQueue.GetLastProcessedOfflocFile);

        dbMessagingService.SubscribeToDbRequest<GetLastProcessedDeliusFile>(async (message) =>
        {
            string? result = await dbInteractionService.GetLastProcessedDeliusFileName();
            dbMessagingService.DbPublishResponse(new ResultGetLastProcessedDeliusFileMessage(result));
        }, TDbQueue.GetLastProcessedDeliusFile);
        
    }
}
