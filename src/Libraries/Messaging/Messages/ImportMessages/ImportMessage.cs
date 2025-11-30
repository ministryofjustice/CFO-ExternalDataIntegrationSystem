using Messaging.Queues;

namespace Messaging.Messages.ImportMessages
{
    public abstract class ImportMessage : Message
    {
        public TImportQueue routingKey { get; set; }
        public string fileName = string.Empty; //To handle processing of multiple files.
        
        public override string Exchange => Exchanges.import;
        public override string RoutingKey => routingKey.ToString();
    }
}