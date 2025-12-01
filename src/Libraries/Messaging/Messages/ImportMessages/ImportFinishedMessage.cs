using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.ImportMessages;

public class ImportFinishedMessage : ImportMessage
{    
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Import Completed");

    [JsonConstructor]
    public ImportFinishedMessage()
    {
        Queue = TImportQueue.ImportFinished;
    }
}