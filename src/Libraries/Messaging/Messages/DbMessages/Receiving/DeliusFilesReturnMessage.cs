using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class DeliusFilesReturnMessage : DbResponseMessage
{
    public string[] fileNames = { };
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Processed Delius files returned.");

    [JsonConstructor]
    public DeliusFilesReturnMessage()
    { }

    public DeliusFilesReturnMessage(string[] fileNames)
    {
        this.fileNames = fileNames;
        Queue = TDbQueue.ReturnedDeliusFiles;
    }
}