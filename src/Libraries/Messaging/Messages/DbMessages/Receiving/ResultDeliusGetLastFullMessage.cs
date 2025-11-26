
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class ResultDeliusGetLastFullMessage : DbResponseMessage
{
    public int fileId;

	public override StatusUpdateMessage StatusMessage =>
		new StatusUpdateMessage();

    [JsonConstructor]
    public ResultDeliusGetLastFullMessage()
    {
        Queue = TDbQueue.ReturnDeliusGetLastFull;
    }

    public ResultDeliusGetLastFullMessage(int fileId) : this()
    {
        this.fileId = fileId;
    }
}
