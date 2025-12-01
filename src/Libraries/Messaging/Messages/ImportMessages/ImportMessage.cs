using Messaging.Queues;

namespace Messaging.Messages.ImportMessages
{
    public abstract class ImportMessage : Message
    {
        public TImportQueue Queue { get; set; }
        public string FileName { get; set; } = string.Empty;
        
        public override string Exchange => Exchanges.import;
        public override string RoutingKey => Queue.ToString();
    }
}