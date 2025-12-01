using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class GetLastProcessedOfflocFileResponse : DbResponseMessage
{
    public string? FileName { get; set; }
    public override StatusUpdateMessage StatusMessage => new();
    public override TDbQueue Queue { get; set; } = TDbQueue.ResultLastProcessedOfflocFile;
}
