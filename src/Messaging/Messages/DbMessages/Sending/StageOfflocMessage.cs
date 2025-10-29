using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class StageOfflocMessage : DbRequestMessage
{
    public string fileName = string.Empty;

    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage();

    [JsonConstructor]
    public StageOfflocMessage()
    {
        Queue = TDbQueue.StageOffloc;
        ReplyQueue = TDbQueue.ResultStageOffloc;
    }

    public StageOfflocMessage(string fileName) : this()
    {
        this.fileName = fileName;
    }
}
