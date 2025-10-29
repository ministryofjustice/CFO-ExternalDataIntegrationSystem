
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class StageDeliusMessage : DbRequestMessage
{
    public string fileName = string.Empty;
    public string filePath = string.Empty;

    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Delius Staging started.");

    [JsonConstructor]
    public StageDeliusMessage()
    {
        Queue = TDbQueue.StageDelius;
        ReplyQueue = TDbQueue.ResultStageDelius;
    }
    
    public StageDeliusMessage(string fileName, string filePath) : this()
    {
        this.fileName = fileName;
        this.filePath = filePath;
    }
}
