using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.StagingMessages;

public class DeliusParserFinishedMessage : StagingMessage
{
    public string filePath = string.Empty;
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Delius parser finished for file {fileName}");
    public bool emptyFile;

    [JsonConstructor]
    public DeliusParserFinishedMessage()
    { }

    public DeliusParserFinishedMessage(string fileName, string filePath,bool emptyFile)
    {
        routingKey = TStagingQueue.DeliusImport;
        this.fileName = fileName;
        this.filePath = filePath;
        this.emptyFile = emptyFile;
    }
}
