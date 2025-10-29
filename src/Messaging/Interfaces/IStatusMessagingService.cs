
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Interfaces;

public interface IStatusMessagingService
{
    void StatusPublish<T>(T Message) where T : StatusUpdateMessage;
    void StatusSubscribe<T>(Action<T> handler, TStatusQueue queue) where T : StatusUpdateMessage;
}
