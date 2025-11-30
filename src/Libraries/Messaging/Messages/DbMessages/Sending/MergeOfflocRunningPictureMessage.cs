using Messaging.Messages.StatusMessages;
using System.Text.Json.Serialization;
using Messaging.Queues;
using Messaging.Messages.MergingMessages;

namespace Messaging.Messages.DbMessages.Sending;

public class MergeOfflocRunningPictureMessage : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Merging into Offloc running picture started.");

    public string FileName { get; set; } = string.Empty;

    [JsonConstructor]
    public MergeOfflocRunningPictureMessage()
    {
        Queue = TDbQueue.MergeOffloc;
        ReplyQueue = TDbQueue.ResultMergeOffloc;
    }

    public MergeOfflocRunningPictureMessage(string fileName) : this()
    {
        FileName = fileName;
    }
}
