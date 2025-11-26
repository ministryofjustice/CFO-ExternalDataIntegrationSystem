using Messaging.Messages.MatchingMessages;
using Messaging.Queues;

namespace Messaging.Interfaces
{
    public interface IMatchingMessagingService
    {
        Task MatchingPublishAsync<T>(T message) where T : MatchingMessage;
        Task MatchingSubscribeAsync<T>(Action<T> handler, TMatchingQueue queue) where T : MatchingMessage;
    }
}
