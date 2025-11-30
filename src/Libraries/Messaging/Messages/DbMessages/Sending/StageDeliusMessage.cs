
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class StageDeliusMessage : DbRequestMessage
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;

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
        FileName = fileName;
        FilePath = filePath;
    }
}
