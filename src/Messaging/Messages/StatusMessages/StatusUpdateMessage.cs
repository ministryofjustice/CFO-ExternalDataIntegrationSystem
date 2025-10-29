
using System.Reflection;
using System.Text.Json.Serialization;
using Messaging.Queues;

namespace Messaging.Messages.StatusMessages;

public class StatusUpdateMessage
{
    public TStatusQueue RoutingKey { get; set; }
    public string Message { get; set; }
    public string Caller { get; set; }

    [JsonConstructor]
    public StatusUpdateMessage() : this(string.Empty) { }

    public StatusUpdateMessage(string message)
    {
        RoutingKey = TStatusQueue.StatusUpdate;
        Message = message;
        Caller = Assembly.GetCallingAssembly().GetName().Name!;
    }
}

