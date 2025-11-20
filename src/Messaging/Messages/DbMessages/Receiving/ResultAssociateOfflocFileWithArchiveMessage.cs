using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class ResultAssociateOfflocFileWithArchiveMessage : DbResponseMessage
{
	public override StatusUpdateMessage StatusMessage => new("Offloc file associated with archive");

    public ResultAssociateOfflocFileWithArchiveMessage()
    {
        Queue = TDbQueue.ResultAssociateOfflocFileWithArchive;
    }
}