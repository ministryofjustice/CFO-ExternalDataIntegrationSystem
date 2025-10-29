
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

        dbMessagingService.DbLongSubscribe<GetDeliusFilesMessage>(async(message) =>
        {
            string[] results = await dbInteractionService.GetProcessedDeliusFileNames();
            DeliusFilesReturnMessage msg = new DeliusFilesReturnMessage(results);
            dbMessagingService.DbPublishResponse(msg);
        }, TDbQueue.GetProcessedDeliusFiles);

        dbMessagingService.DbLongSubscribe<GetOfflocFilesMessage>(async(message) =>
        {
            string[] results = await dbInteractionService.GetProcessedOfflocFileNames();
            dbMessagingService.DbPublishResponse(new OfflocFilesReturnMessage(results));
        }, TDbQueue.GetProcessedOfflocFiles);

        dbMessagingService.DbLongSubscribe<StageDeliusMessage>(async (message) =>
        {
            await dbInteractionService.StageDelius(message.fileName, message.filePath);
            dbMessagingService.DbPublishResponse(new StageDeliusReturnMessage());
            mergingService.MergingPublish(new DeliusFilesCleanupMessage(message.fileName));
        }, TDbQueue.StageDelius);

        dbMessagingService.DbLongSubscribe<MergeDeliusRunningPictureMessage>(async (message) =>
        {
            //await dbInteractionService.StandardiseDeliusStaging();
            await dbInteractionService.MergeDeliusPicture();
            await dbInteractionService.ClearDeliusStaging();

			dbMessagingService.DbPublishResponse(new MergeDeliusReturnMessage());
		}, TDbQueue.MergeDelius);

		dbMessagingService.DbLongSubscribe<ClearDeliusStaging>(async (message) =>
		{
			await dbInteractionService.ClearDeliusStaging();
			dbMessagingService.DbPublishResponse(new ResultClearDeliusStaging());
		}, TDbQueue.ClearDeliusStaging);

		dbMessagingService.DbLongSubscribe<StageOfflocMessage>(async (message) =>
        {
            await dbInteractionService.StageOffloc(message.fileName);
            dbMessagingService.DbPublishResponse(new StageOfflocReturnMessage());

            mergingService.MergingPublish(new OfflocFilesCleanupMessage(message.fileName));       
        }, TDbQueue.StageOffloc);

        dbMessagingService.DbLongSubscribe<MergeOfflocRunningPictureMessage>(async (message) =>
        {
            await dbInteractionService.MergeOfflocPicture();
            await dbInteractionService.ClearOfflocStaging();

            dbMessagingService.DbPublishResponse(new MergeOfflocReturnMessage());

        }, TDbQueue.MergeOffloc);

        dbMessagingService.DbLongSubscribe<ClearOfflocStaging>(async (message) =>
        {
            await dbInteractionService.ClearOfflocStaging();
            dbMessagingService.DbPublishResponse(new ResultClearOfflocStaging());
        }, TDbQueue.ClearOfflocStaging);

        dbMessagingService.DbLongSubscribe<DeliusGetLastFullMessage>(async (message) =>
        {
            int res = await dbInteractionService.DeliusGetFileIdLastFull();
            dbMessagingService.DbPublishResponse(new ResultDeliusGetLastFullMessage(res));
        }, TDbQueue.DeliusGetLastFullId);
    }
}
