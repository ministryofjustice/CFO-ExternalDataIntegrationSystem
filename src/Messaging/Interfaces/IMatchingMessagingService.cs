using Messaging.Messages.MatchingMessages;
using Messaging.Queues;

namespace Messaging.Interfaces
{
    public interface IMatchingMessagingService
    {
        void MatchingPublish<T>(T message) where T : MatchingMessage;
        void MatchingSubscribe<T>(Action<T> handler, TMatchingQueue queue) where T : MatchingMessage;
    }
}
