using Messaging.Messages.BlockingMessages;
using Messaging.Queues;

namespace Messaging.Interfaces
{
    public interface IBlockingMessagingService
    {
        void BlockingPublish<T>(T message) where T : BlockingMessage;
        void BlockingSubscribe<T>(Action<T> handler, TBlockingQueue queue) where T : BlockingMessage;
    }
}