
using DbInteractions.Services;
using Messaging.Interfaces;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.MergingMessages.CleanupMessages;
using Messaging.Queues;
using Microsoft.Extensions.Hosting;

namespace DbInteractions;

//This class links the messaging service and dbInteractionService- the dbinteraction service does all of the work. 
public class DbBackgroundService(
    IDbInteractionService dbInteractionService,
    IMessageService messageService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageService.SubscribeAsync<GetDeliusFilesMessage>(async (message) =>
        {
            string[] results = await dbInteractionService.GetProcessedDeliusFileNames();
            DeliusFilesReturnMessage msg = new DeliusFilesReturnMessage(results);
            await messageService.DbPublishResponseAsync(msg);
        }, TDbQueue.GetProcessedDeliusFiles);

        await messageService.SubscribeAsync<GetOfflocFilesMessage>(async (message) =>
        {
            string[] results = await dbInteractionService.GetProcessedOfflocFileNames();
            await messageService.DbPublishResponseAsync(new OfflocFilesReturnMessage(results));
        }, TDbQueue.GetProcessedOfflocFiles);

        await messageService.SubscribeAsync<StageDeliusMessage>(async (message) =>
        {
            await dbInteractionService.StageDelius(message.FileName, message.FilePath);
            await messageService.DbPublishResponseAsync(new StageDeliusReturnMessage());
            await messageService.PublishAsync(new DeliusFilesCleanupMessage(message.FileName));
        }, TDbQueue.StageDelius);

        await messageService.SubscribeAsync<MergeDeliusRunningPictureMessage>(async (message) =>
        {
            await dbInteractionService.MergeDeliusPicture(message.FileName);
            await dbInteractionService.ClearDeliusStaging();
            await messageService.DbPublishResponseAsync(new MergeDeliusReturnMessage());
        }, TDbQueue.MergeDelius);

        await messageService.SubscribeAsync<ClearDeliusStaging>(async (message) =>
        {
            await dbInteractionService.ClearDeliusStaging();
            await messageService.DbPublishResponseAsync(new ResultClearDeliusStaging());
        }, TDbQueue.ClearDeliusStaging);

        await messageService.SubscribeAsync<StageOfflocMessage>(async (message) =>
        {
            await dbInteractionService.StageOffloc(message.FileName);
            await messageService.DbPublishResponseAsync(new StageOfflocReturnMessage());
            await messageService.PublishAsync(new OfflocFilesCleanupMessage(message.FileName));
        }, TDbQueue.StageOffloc);

        await messageService.SubscribeAsync<MergeOfflocRunningPictureMessage>(async (message) =>
        {
            await dbInteractionService.MergeOfflocPicture(message.FileName);
            await dbInteractionService.ClearOfflocStaging();
            await messageService.DbPublishResponseAsync(new MergeOfflocReturnMessage());
        }, TDbQueue.MergeOffloc);

        await messageService.SubscribeAsync<ClearOfflocStaging>(async (message) =>
        {
            await dbInteractionService.ClearOfflocStaging();
            await messageService.DbPublishResponseAsync(new ResultClearOfflocStaging());
        }, TDbQueue.ClearOfflocStaging);

        await messageService.SubscribeAsync<DeliusGetLastFullMessage>(async (message) =>
        {
            int res = await dbInteractionService.DeliusGetFileIdLastFull();
            await messageService.DbPublishResponseAsync(new ResultDeliusGetLastFullMessage(res));
        }, TDbQueue.DeliusGetLastFullId);

        await messageService.SubscribeAsync<OfflocFileProcessingStarted>(async (message) =>
        {
            await dbInteractionService.CreateOfflocProcessedFileEntry(message.FileName, message.FileId, message.ArchiveName);
            await messageService.DbPublishResponseAsync(new ResultOfflocFileProcessingStarted());
        }, TDbQueue.OfflocFileProcessingStarted);

        await messageService.SubscribeAsync<DeliusFileProcessingStarted>(async (message) =>
        {
            await dbInteractionService.CreateDeliusProcessedFileEntry(message.FileName, message.FileId);
            await messageService.DbPublishResponseAsync(new ResultDeliusFileProcessingStarted());
        }, TDbQueue.DeliusFileProcessingStarted);

        await messageService.SubscribeAsync<AssociateOfflocFileWithArchiveMessage>(async (message) =>
        {
            await dbInteractionService.AssociateOfflocFileWithArchive(message.FileName, message.ArchiveName);
            await messageService.DbPublishResponseAsync(new ResultAssociateOfflocFileWithArchiveMessage());
        }, TDbQueue.AssociateOfflocFileWithArchive);

        await messageService.SubscribeAsync<IsDeliusReadyForProcessingMessage>(async (message) =>
        {
            bool result = await dbInteractionService.IsDeliusReadyForProcessing();
            await messageService.DbPublishResponseAsync(new IsDeliusReadyForProcessingReturnMessage(result));
        }, TDbQueue.IsDeliusReadyForProcessing);
        
        await messageService.SubscribeAsync<IsOfflocReadyForProcessingMessage>(async (message) =>
        {
            bool result = await dbInteractionService.IsOfflocReadyForProcessing();
            await messageService.DbPublishResponseAsync(new IsOfflocReadyForProcessingReturnMessage(result));
        }, TDbQueue.IsOfflocReadyForProcessing);

        await messageService.SubscribeAsync<GetLastProcessedOfflocFile>(async (message) =>
        {
            string? result = await dbInteractionService.GetLastProcessedOfflocFileName();
            await messageService.DbPublishResponseAsync(new ResultGetLastProcessedOfflocFileMessage(result));
        }, TDbQueue.GetLastProcessedOfflocFile);

        await messageService.SubscribeAsync<GetLastProcessedDeliusFile>(async (message) =>
        {
            string? result = await dbInteractionService.GetLastProcessedDeliusFileName();
            await messageService.DbPublishResponseAsync(new ResultGetLastProcessedDeliusFileMessage(result));
        }, TDbQueue.GetLastProcessedDeliusFile);
        
    }
}
