using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class AssociateOfflocFileWithArchiveMessage : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage => new();

    public string FileName { get; set; } = string.Empty;
    public string ArchiveName { get; set; } = string.Empty;

    [JsonConstructor]
    public AssociateOfflocFileWithArchiveMessage()
    {
        Queue = TDbQueue.AssociateOfflocFileWithArchive;
        ReplyQueue = TDbQueue.ResultAssociateOfflocFileWithArchive;
    }

    public AssociateOfflocFileWithArchiveMessage(string fileName, string archiveName) : this()
    {
        FileName = fileName;
        ArchiveName = archiveName;
    }
}