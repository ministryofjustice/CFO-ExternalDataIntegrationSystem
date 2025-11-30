
using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.StagingMessages;

public class OfflocCleanerFinishedMessage : StagingMessage
{
    public string[] filesToParse { get; init; } = { };
    public int[] fieldsToIgnore = { }; //Sent in message to avoid passing complex configuration seperately.

    public override StatusUpdateMessage StatusMessage => new 
        StatusUpdateMessage($"Offloc Cleaner finished for file(s) { string.Join(',', filesToParse) }.");

    [JsonConstructor]
    public OfflocCleanerFinishedMessage()
    {
        Queue = TStagingQueue.OfflocParser;
    }
    //For parallel processing.
    public OfflocCleanerFinishedMessage(string[] filesToParse, int[] redundantFields)
        : this()
    {
        this.filesToParse = filesToParse;
        this.fieldsToIgnore = redundantFields;
    }
    ////For sequential processing.
    public OfflocCleanerFinishedMessage(string fileToParse, int[] redundantFields, string fileName)
        : this(new string[] { fileToParse }, redundantFields)
    {
        FileName = fileName;
    }
}
