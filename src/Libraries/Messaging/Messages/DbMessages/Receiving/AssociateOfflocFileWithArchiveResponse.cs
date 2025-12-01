using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class AssociateOfflocFileWithArchiveResponse : DbResponseMessage
{
	public override StatusUpdateMessage StatusMessage => new("Offloc file associated with archive");

    public AssociateOfflocFileWithArchiveResponse()
    {
        Queue = TDbQueue.ResultAssociateOfflocFileWithArchive;
    }
}