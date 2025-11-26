
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class DeliusGetLastFullMessage : DbRequestMessage
{
	public override StatusUpdateMessage StatusMessage =>
		new StatusUpdateMessage();

    [JsonConstructor]
    public DeliusGetLastFullMessage()
    {
        Queue = TDbQueue.DeliusGetLastFullId;
        ReplyQueue = TDbQueue.ReturnDeliusGetLastFull;
    }
}
