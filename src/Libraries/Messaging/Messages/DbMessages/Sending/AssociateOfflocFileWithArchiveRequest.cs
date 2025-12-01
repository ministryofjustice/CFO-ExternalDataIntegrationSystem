using System.Text.Json.Serialization;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class AssociateOfflocFileWithArchiveRequest : DbRequestMessage<AssociateOfflocFileWithArchiveResponse>
{
    public override StatusUpdateMessage StatusMessage => new();

    public string FileName { get; set; } = string.Empty;
    public string ArchiveName { get; set; } = string.Empty;

    [JsonConstructor]
    public AssociateOfflocFileWithArchiveRequest()
    {
        Queue = TDbQueue.AssociateOfflocFileWithArchive;
    }

    public AssociateOfflocFileWithArchiveRequest(string fileName, string archiveName) : this()
    {
        FileName = fileName;
        ArchiveName = archiveName;
    }
}