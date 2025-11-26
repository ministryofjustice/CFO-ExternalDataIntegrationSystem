using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class ResultGetLastProcessedDeliusFileMessage : DbResponseMessage
{
    public string? fileName;

	public override StatusUpdateMessage StatusMessage => new();

    [JsonConstructor]
    public ResultGetLastProcessedDeliusFileMessage()
    {
        Queue = TDbQueue.ResultLastProcessedDeliusFile;
    }

    public ResultGetLastProcessedDeliusFileMessage(string? fileName) : this()
    {
        this.fileName = fileName;
    }
}
