using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class ResultGetLastProcessedDeliusFileMessage : DbResponseMessage
{
    public string? FileName { get; set; }

	public override StatusUpdateMessage StatusMessage => new();

    [JsonConstructor]
    public ResultGetLastProcessedDeliusFileMessage()
    {
        Queue = TDbQueue.ResultLastProcessedDeliusFile;
    }

    public ResultGetLastProcessedDeliusFileMessage(string? fileName) : this()
    {
        FileName = fileName;
    }
}
