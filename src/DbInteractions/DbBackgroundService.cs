
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
public class DbBackgroundService(
    IDbInteractionService dbInteractionService,
    IDbMessagingService dbMessagingService,
    IMessageService messageService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await dbMessagingService.SubscribeToDbRequestAsync<GetDeliusFilesMessage>(async (message) =>
        {
            string[] results = await dbInteractionService.GetProcessedDeliusFileNames();
            DeliusFilesReturnMessage msg = new DeliusFilesReturnMessage(results);
            await dbMessagingService.DbPublishResponseAsync(msg);
        }, TDbQueue.GetProcessedDeliusFiles);

        await dbMessagingService.SubscribeToDbRequestAsync<GetOfflocFilesMessage>(async (message) =>
        {
            string[] results = await dbInteractionService.GetProcessedOfflocFileNames();
            await dbMessagingService.DbPublishResponseAsync(new OfflocFilesReturnMessage(results));
        }, TDbQueue.GetProcessedOfflocFiles);

        await dbMessagingService.SubscribeToDbRequestAsync<StageDeliusMessage>(async (message) =>
        {
            await dbInteractionService.StageDelius(message.FileName, message.FilePath);
            await dbMessagingService.DbPublishResponseAsync(new StageDeliusReturnMessage());
            await messageService.PublishAsync(new DeliusFilesCleanupMessage(message.FileName));
        }, TDbQueue.StageDelius);

        await dbMessagingService.SubscribeToDbRequestAsync<MergeDeliusRunningPictureMessage>(async (message) =>
        {
            await dbInteractionService.MergeDeliusPicture(message.FileName);
            await dbInteractionService.ClearDeliusStaging();

            await dbMessagingService.DbPublishResponseAsync(new MergeDeliusReturnMessage());
        }, TDbQueue.MergeDelius);

        await dbMessagingService.SubscribeToDbRequestAsync<ClearDeliusStaging>(async (message) =>
        {
            await dbInteractionService.ClearDeliusStaging();
            await dbMessagingService.DbPublishResponseAsync(new ResultClearDeliusStaging());
        }, TDbQueue.ClearDeliusStaging);

        await dbMessagingService.SubscribeToDbRequestAsync<StageOfflocMessage>(async (message) =>
        {
            await dbInteractionService.StageOffloc(message.FileName);
            await dbMessagingService.DbPublishResponseAsync(new StageOfflocReturnMessage());

            await messageService.PublishAsync(new OfflocFilesCleanupMessage(message.FileName));
        }, TDbQueue.StageOffloc);

        await dbMessagingService.SubscribeToDbRequestAsync<MergeOfflocRunningPictureMessage>(async (message) =>
        {
            await dbInteractionService.MergeOfflocPicture(message.FileName);
            await dbInteractionService.ClearOfflocStaging();
            await dbMessagingService.DbPublishResponseAsync(new MergeOfflocReturnMessage());

        }, TDbQueue.MergeOffloc);

        await dbMessagingService.SubscribeToDbRequestAsync<ClearOfflocStaging>(async (message) =>
        {
            await dbInteractionService.ClearOfflocStaging();
            await dbMessagingService.DbPublishResponseAsync(new ResultClearOfflocStaging());
        }, TDbQueue.ClearOfflocStaging);

        await dbMessagingService.SubscribeToDbRequestAsync<DeliusGetLastFullMessage>(async (message) =>
        {
            int res = await dbInteractionService.DeliusGetFileIdLastFull();
            await dbMessagingService.DbPublishResponseAsync(new ResultDeliusGetLastFullMessage(res));
        }, TDbQueue.DeliusGetLastFullId);

        await dbMessagingService.SubscribeToDbRequestAsync<OfflocFileProcessingStarted>(async (message) =>
        {
            await dbInteractionService.CreateOfflocProcessedFileEntry(message.FileName, message.FileId, message.ArchiveName);
            await dbMessagingService.DbPublishResponseAsync(new ResultOfflocFileProcessingStarted());
        }, TDbQueue.OfflocFileProcessingStarted);

        await dbMessagingService.SubscribeToDbRequestAsync<DeliusFileProcessingStarted>(async (message) =>
        {
            await dbInteractionService.CreateDeliusProcessedFileEntry(message.FileName, message.FileId);
            await dbMessagingService.DbPublishResponseAsync(new ResultDeliusFileProcessingStarted());
        }, TDbQueue.DeliusFileProcessingStarted);

        await dbMessagingService.SubscribeToDbRequestAsync<AssociateOfflocFileWithArchiveMessage>(async (message) =>
        {
            await dbInteractionService.AssociateOfflocFileWithArchive(message.FileName, message.ArchiveName);
            await dbMessagingService.DbPublishResponseAsync(new ResultAssociateOfflocFileWithArchiveMessage());
        }, TDbQueue.AssociateOfflocFileWithArchive);

        await dbMessagingService.SubscribeToDbRequestAsync<IsDeliusReadyForProcessingMessage>(async (message) =>
        {
            bool result = await dbInteractionService.IsDeliusReadyForProcessing();
            await dbMessagingService.DbPublishResponseAsync(new IsDeliusReadyForProcessingReturnMessage(result));
        }, TDbQueue.IsDeliusReadyForProcessing);
        
        
        await dbMessagingService.SubscribeToDbRequestAsync<IsOfflocReadyForProcessingMessage>(async (message) =>
        {
            bool result = await dbInteractionService.IsOfflocReadyForProcessing();
            await dbMessagingService.DbPublishResponseAsync(new IsOfflocReadyForProcessingReturnMessage(result));
        }, TDbQueue.IsOfflocReadyForProcessing);

        await dbMessagingService.SubscribeToDbRequestAsync<GetLastProcessedOfflocFile>(async (message) =>
        {
            string? result = await dbInteractionService.GetLastProcessedOfflocFileName();
            await dbMessagingService.DbPublishResponseAsync(new ResultGetLastProcessedOfflocFileMessage(result));
        }, TDbQueue.GetLastProcessedOfflocFile);

        await dbMessagingService.SubscribeToDbRequestAsync<GetLastProcessedDeliusFile>(async (message) =>
        {
            string? result = await dbInteractionService.GetLastProcessedDeliusFileName();
            await dbMessagingService.DbPublishResponseAsync(new ResultGetLastProcessedDeliusFileMessage(result));
        }, TDbQueue.GetLastProcessedDeliusFile);
        
    }
}
