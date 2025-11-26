using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class ResultGetLastProcessedOfflocFileMessage : DbResponseMessage
{
    public string? fileName;

	public override StatusUpdateMessage StatusMessage => new();

    [JsonConstructor]
    public ResultGetLastProcessedOfflocFileMessage()
    {
        Queue = TDbQueue.ResultLastProcessedOfflocFile;
    }

    public ResultGetLastProcessedOfflocFileMessage(string? fileName) : this()
    {
        this.fileName = fileName;
    }
}
