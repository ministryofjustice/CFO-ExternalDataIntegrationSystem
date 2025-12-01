using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.StagingMessages;
public class OfflocParserFinishedMessage : StagingMessage
{
    public string FilePath { get; set; } = string.Empty;
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Offloc Parser finished for file {FilePath.Split('/').Last()}.");
    public bool EmptyFile { get; set; }

    [JsonConstructor]
    public OfflocParserFinishedMessage()
    {
        Queue = TStagingQueue.OfflocImport;
    }

    public OfflocParserFinishedMessage(string filePath, bool emptyFile) : this()
    {
        FilePath = filePath;
        EmptyFile = emptyFile;
    }
}
