using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class GetOfflocFilesMessage : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Sent request to get processed offloc files.");

    [JsonConstructor]
    public GetOfflocFilesMessage()
    {
        Queue = TDbQueue.GetProcessedOfflocFiles;
        ReplyQueue = TDbQueue.ReturnedOfflocFiles;
    }
}
