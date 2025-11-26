
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Interfaces;

public interface IStatusMessagingService
{
    Task StatusPublishAsync<T>(T Message) where T : StatusUpdateMessage;
    Task StatusSubscribeAsync<T>(Action<T> handler, TStatusQueue queue) where T : StatusUpdateMessage;
}
