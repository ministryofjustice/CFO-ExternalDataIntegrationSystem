
using Messaging.Messages.MergingMessages;
using Messaging.Queues;

namespace Messaging.Interfaces;

public interface IMergingMessagingService
{
    Task MergingPublishAsync<T>(T message) where T : MergingMessage;
    Task MergingSubscribeAsync<T>(Action<T> handler, TMergingQueue queue) where T : MergingMessage;
}
