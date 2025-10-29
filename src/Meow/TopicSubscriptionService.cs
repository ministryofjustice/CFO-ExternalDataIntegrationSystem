using Rebus.Bus;

namespace Meow;

public class TopicSubscriptionService(IBus bus) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await bus.Advanced.Topics.Subscribe("Cfo.Cats.Application.Features.Participants.IntegrationEvents.ParticipantCreatedIntegrationEvent, Cfo.Cats.Application");
    }
}
