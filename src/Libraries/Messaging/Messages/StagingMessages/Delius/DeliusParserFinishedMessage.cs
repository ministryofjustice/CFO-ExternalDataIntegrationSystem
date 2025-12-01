using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.StagingMessages;

public class DeliusParserFinishedMessage : StagingMessage
{
    public string FilePath { get; set; } = string.Empty;
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Delius parser finished for file {FileName}");
    public bool EmptyFile { get; set; }

    [JsonConstructor]
    public DeliusParserFinishedMessage()
    {
        Queue = TStagingQueue.DeliusImport;
    }

    public DeliusParserFinishedMessage(string fileName, string filePath, bool emptyFile) : this()
    {
        FileName = fileName;
        FilePath = filePath;
        EmptyFile = emptyFile;
    }
}
