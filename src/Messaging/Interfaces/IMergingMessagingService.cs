
using Messaging.Messages.MergingMessages;
using Messaging.Queues;

namespace Messaging.Interfaces;

//Interfaces are split up so microservices can access a sub-section of the messaging service. 
public interface IMergingMessagingService
{
    void MergingPublish<T>(T message) where T : MergingMessage;
    void MergingSubscribe<T>(Action<T> handler, TMergingQueue queue) where T : MergingMessage;
}
