using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class AssociateOfflocFileWithArchiveMessage : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage => new();

    public string fileName { get; set; }
    public string archiveName { get; set; }

    [JsonConstructor]
    public AssociateOfflocFileWithArchiveMessage(string fileName, string archiveName)
    {
        Queue = TDbQueue.AssociateOfflocFileWithArchive;
        ReplyQueue = TDbQueue.ResultAssociateOfflocFileWithArchive;
        this.fileName = fileName;
        this.archiveName = archiveName;
    }
}