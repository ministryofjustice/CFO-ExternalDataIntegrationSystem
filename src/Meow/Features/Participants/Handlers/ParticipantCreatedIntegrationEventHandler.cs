using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Meow.Features.Participants.Handlers;

public class ParticipantCreatedIntegrationEventHandler(
    ILogger<ParticipantCreatedIntegrationEventHandler> logger,
    ClusteringContext context) : IHandleMessages<ParticipantCreatedIntegrationEvent>
{
    public async Task Handle(ParticipantCreatedIntegrationEvent e)
    {
        logger.LogDebug($"Handling {nameof(ParticipantCreatedIntegrationEvent)}...");

        var cluster = await context.Clusters
             .Include(c => c.Members)
             .SingleOrDefaultAsync(c => c.UPCI == e.ParticipantId) ?? throw new KeyNotFoundException(e.ParticipantId);

        // Update timestamp
        cluster.IdentifiedOn = e.OccurredOn;

        var primaryRecord = cluster.Members
            .SingleOrDefault(m => m.NodeKey == e.PrimaryRecordKeyAtCreation) ?? throw new KeyNotFoundException($"{e.PrimaryRecordKeyAtCreation} in {e.ParticipantId}");

        // HardLink primary record
        primaryRecord.HardLink = true;

        await context.SaveChangesAsync();
    }

}
