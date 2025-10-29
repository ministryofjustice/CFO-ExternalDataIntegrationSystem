
using Messaging.Messages.StagingMessages;
using Messaging.Queues;

namespace Messaging.Interfaces;

public interface IStagingMessagingService
{
    void StagingPublish<T>(T message) where T : StagingMessage;
    void StagingSubscribe<T>(Action<T> handler, TStagingQueue queue) where T : StagingMessage;
}
