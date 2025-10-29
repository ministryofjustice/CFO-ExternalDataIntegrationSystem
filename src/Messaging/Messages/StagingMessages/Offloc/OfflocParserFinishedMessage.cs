using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.StagingMessages;
public class OfflocParserFinishedMessage : StagingMessage
{
    public string filePath = string.Empty;
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Offloc Parser finished for file {filePath.Split('/').Last()}.");
    public bool emptyFile;

    [JsonConstructor]
    public OfflocParserFinishedMessage()
    { }

    public OfflocParserFinishedMessage(string filePath,bool emptyFile)
    {
        routingKey = TStagingQueue.OfflocImport;
        this.filePath = filePath;
        this.emptyFile = emptyFile;
    }
}
