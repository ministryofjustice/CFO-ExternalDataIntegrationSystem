
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
        await messageService.SubscribeAsync<GetDeliusFilesRequest>(async (message) =>
        {
            string[] results = await dbInteractionService.GetProcessedDeliusFileNames();
            GetDeliusFilesResponse msg = new GetDeliusFilesResponse { FileNames = results };
            await messageService.PublishAsync(msg);
        }, TDbQueue.GetProcessedDeliusFiles);

        await messageService.SubscribeAsync<GetOfflocFilesRequest>(async (message) =>
        {
            string[] results = await dbInteractionService.GetProcessedOfflocFileNames();
            await messageService.PublishAsync(new GetOfflocFilesResponse { OfflocFiles = results });
        }, TDbQueue.GetProcessedOfflocFiles);

        await messageService.SubscribeAsync<StageDeliusRequest>(async (message) =>
        {
            await dbInteractionService.StageDelius(message.FileName, message.FilePath);
            await messageService.PublishAsync(new StageDeliusResponse());
            await messageService.PublishAsync(new DeliusFilesCleanupMessage(message.FileName));
        }, TDbQueue.StageDelius);

        await messageService.SubscribeAsync<MergeDeliusRequest>(async (message) =>
        {
            await dbInteractionService.MergeDeliusPicture(message.FileName);
            await dbInteractionService.ClearDeliusStaging();
            await messageService.PublishAsync(new MergeDeliusResponse());
        }, TDbQueue.MergeDelius);

        await messageService.SubscribeAsync<ClearDeliusStagingRequest>(async (message) =>
        {
            await dbInteractionService.ClearDeliusStaging();
            await messageService.PublishAsync(new ClearDeliusStagingResponse());
        }, TDbQueue.ClearDeliusStaging);

        await messageService.SubscribeAsync<StageOfflocRequest>(async (message) =>
        {
            await dbInteractionService.StageOffloc(message.FileName);
            await messageService.PublishAsync(new StageOfflocResponse());
            await messageService.PublishAsync(new OfflocFilesCleanupMessage(message.FileName));
        }, TDbQueue.StageOffloc);

        await messageService.SubscribeAsync<MergeOfflocRequest>(async (message) =>
        {
            await dbInteractionService.MergeOfflocPicture(message.FileName);
            await dbInteractionService.ClearOfflocStaging();
            await messageService.PublishAsync(new MergeOfflocResponse());
        }, TDbQueue.MergeOffloc);

        await messageService.SubscribeAsync<ClearOfflocStagingRequest>(async (message) =>
        {
            await dbInteractionService.ClearOfflocStaging();
            await messageService.PublishAsync(new ClearOfflocStagingResponse());
        }, TDbQueue.ClearOfflocStaging);

        await messageService.SubscribeAsync<StartOfflocFileProcessingRequest>(async (message) =>
        {
            await dbInteractionService.CreateOfflocProcessedFileEntry(message.FileName, message.FileId, message.ArchiveName);
            await messageService.PublishAsync(new StartOfflocFileProcessingResponse());
        }, TDbQueue.OfflocFileProcessingStarted);

        await messageService.SubscribeAsync<StartDeliusFileProcessingRequest>(async (message) =>
        {
            await dbInteractionService.CreateDeliusProcessedFileEntry(message.FileName, message.FileId);
            await messageService.PublishAsync(new StartDeliusFileProcessingResponse());
        }, TDbQueue.DeliusFileProcessingStarted);

        await messageService.SubscribeAsync<AssociateOfflocFileWithArchiveRequest>(async (message) =>
        {
            await dbInteractionService.AssociateOfflocFileWithArchive(message.FileName, message.ArchiveName);
            await messageService.PublishAsync(new AssociateOfflocFileWithArchiveResponse());
        }, TDbQueue.AssociateOfflocFileWithArchive);

        await messageService.SubscribeAsync<CheckDeliusReadyRequest>(async (message) =>
        {
            bool result = await dbInteractionService.IsDeliusReadyForProcessing();
            await messageService.PublishAsync(new CheckDeliusReadyResponse { IsReady = result });
        }, TDbQueue.IsDeliusReadyForProcessing);
        
        await messageService.SubscribeAsync<CheckOfflocReadyRequest>(async (message) =>
        {
            bool result = await dbInteractionService.IsOfflocReadyForProcessing();
            await messageService.PublishAsync(new CheckOfflocReadyResponse { IsReady = result });
        }, TDbQueue.IsOfflocReadyForProcessing);

        await messageService.SubscribeAsync<GetLastProcessedOfflocFileRequest>(async (message) =>
        {
            string? result = await dbInteractionService.GetLastProcessedOfflocFileName();
            await messageService.PublishAsync(new GetLastProcessedOfflocFileResponse { FileName = result });
        }, TDbQueue.GetLastProcessedOfflocFile);

        await messageService.SubscribeAsync<GetLastProcessedDeliusFileRequest>(async (message) =>
        {
            string? result = await dbInteractionService.GetLastProcessedDeliusFileName();
            await messageService.PublishAsync(new GetLastProcessedDeliusFileResponse { FileName = result });
        }, TDbQueue.GetLastProcessedDeliusFile);
        
    }
}
