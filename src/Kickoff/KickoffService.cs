using Messaging.Interfaces;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StagingMessages.Delius;
using Messaging.Messages.StagingMessages.Offloc;
using Messaging.Messages.StatusMessages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace Kickoff;

public class KickoffService(
	IStagingMessagingService messageService, 
	IStatusMessagingService statusService,
	IDbMessagingService dbService,
    IHostApplicationLifetime lifetime) : IHostedService
{

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        LogStatus("Performing pre-kickoff tasks...");

        // await PreKickoffTasks();

        // LogStatus("All pre-kickoff tasks complete. Begin staging message publish...");

        // StagingMessage offlocStagingMessage = new OfflocDownloadFinished();
        // StagingMessage deliusStagingMessage = new DeliusDownloadFinishedMessage();

        // LogStatus($"Publishing {offlocStagingMessage.GetType().Name}...");
        // messageService.StagingPublish(offlocStagingMessage);

        // LogStatus($"Publishing {deliusStagingMessage.GetType().Name}...");
        // messageService.StagingPublish(deliusStagingMessage);

        lifetime.StopApplication();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            LogStatus("Kickoff complete.");
        }, cancellationToken);
    }
    
    private async Task PreKickoffTasks()
    {
        LogStatus("Publishing pre-kickoff messages...");
        messageService.StagingPublish(new ClearHalfCleanedOfflocFiles());
        messageService.StagingPublish(new ClearTemporaryDeliusFiles());

        LogStatus("Pre-kickoff messages published. Beginning staging database tear down...");
        await dbService.DbTransientSubscribe<ClearDeliusStaging, ResultClearDeliusStaging>(new ClearDeliusStaging());
        await dbService.DbTransientSubscribe<ClearOfflocStaging, ResultClearOfflocStaging>(new ClearOfflocStaging());
        LogStatus("Staging database tear down complete.");
    }

    void LogStatus(string message)
    {
        Log.Information(message);
        statusService.StatusPublish(new StatusUpdateMessage(message));
    }

}
