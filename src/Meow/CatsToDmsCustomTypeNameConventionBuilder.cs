using Rebus.Serialization;

namespace Meow;

public class CatsToDmsCustomTypeNameConventionBuilder : IMessageTypeNameConvention
{
    public string GetTypeName(Type type)
    {
        return type.Name;
    }

    public Type GetType(string name)
    {
        return name switch
        {
            var s when s.Contains(nameof(ParticipantCreatedIntegrationEvent)) =>
                typeof(ParticipantCreatedIntegrationEvent),
            _ => Type.GetType(name) ?? throw new ArgumentException($"Unknown type {name}")
        };
    }
}