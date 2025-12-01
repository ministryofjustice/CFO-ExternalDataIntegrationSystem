using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class StageOfflocRequest : DbRequestMessage<StageOfflocResponse>
{
    public string FileName { get; set; } = string.Empty;

    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage();

    [JsonConstructor]
    public StageOfflocRequest()
    {
        Queue = TDbQueue.StageOffloc;
    }

    public StageOfflocRequest(string fileName) : this()
    {
        FileName = fileName;
    }
}
