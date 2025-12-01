
using System.Reflection;
using System.Text.Json.Serialization;
using Messaging.Queues;

namespace Messaging.Messages.StatusMessages;

public class StatusUpdateMessage : IMessage
{
    public virtual string RoutingKey { get; } = TStatusQueue.StatusUpdate.ToString();
    public string Message { get; set; }
    public string Caller { get; set; }

    public string Exchange => Exchanges.status;

    [JsonConstructor]
    public StatusUpdateMessage() : this(string.Empty) { }

    public StatusUpdateMessage(string message)
    {
        Message = message;
        Caller = Assembly.GetCallingAssembly().GetName().Name!;
    }
}

