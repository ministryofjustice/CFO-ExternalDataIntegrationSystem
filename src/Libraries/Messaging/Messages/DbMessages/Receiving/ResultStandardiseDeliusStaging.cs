using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Messages.DbMessages.Receiving
{
    internal class ResultStandardiseDeliusStaging : DbResponseMessage
    {
        public override StatusUpdateMessage StatusMessage =>
            new StatusUpdateMessage("Delius staging standardisation complete.");

        public ResultStandardiseDeliusStaging()
        {
            Queue = TDbQueue.ResultStandardiseDelius;
        }
    }
}
