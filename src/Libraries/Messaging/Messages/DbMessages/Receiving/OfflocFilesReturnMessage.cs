
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class OfflocFilesReturnMessage : DbResponseMessage
{
    public string[] offlocFiles = new string[] { };
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Processed Offloc files returned.");

    [JsonConstructor]
    public OfflocFilesReturnMessage()
    { }
    public OfflocFilesReturnMessage(string[] offlocFiles)
    {
        this.offlocFiles = offlocFiles;
        Queue = TDbQueue.ReturnedOfflocFiles;
    }
}
