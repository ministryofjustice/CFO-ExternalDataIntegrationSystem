using Messaging.Messages.BlockingMessages;
using Messaging.Queues;

namespace Messaging.Interfaces
{
    public interface IBlockingMessagingService
    {
        Task BlockingPublishAsync<T>(T message) where T : BlockingMessage;
        Task BlockingSubscribeAsync<T>(Action<T> handler, TBlockingQueue queue) where T : BlockingMessage;
    }
}