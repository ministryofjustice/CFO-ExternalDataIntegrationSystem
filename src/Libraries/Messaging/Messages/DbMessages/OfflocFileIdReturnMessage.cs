using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages;

public class OfflocFileIdReturnMessage : DbResponseMessage
{
    public DateOnly[] fileDates = Array.Empty<DateOnly>();

    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Processed Offloc files returned.");
    
    [JsonConstructor]
    public OfflocFileIdReturnMessage()
    {
        Queue = TDbQueue.ReturnedOfflocFileDates;
    }

    public OfflocFileIdReturnMessage(DateOnly[] fileIds) : this()
    {
        this.fileDates = fileIds; 
    }
}