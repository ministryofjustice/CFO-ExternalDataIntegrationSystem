
using Messaging.Messages.StagingMessages;
using Messaging.Queues;

namespace Messaging.Interfaces;

public interface IStagingMessagingService
{
    Task StagingPublishAsync<T>(T message) where T : StagingMessage;
    Task StagingSubscribeAsync<T>(Action<T> handler, TStagingQueue queue) where T : StagingMessage;
}
