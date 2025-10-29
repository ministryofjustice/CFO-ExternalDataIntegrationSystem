
using System.Xml.Serialization;

namespace Offloc.Parser.Services.TrimmerContext.CoreContexts;

public class CompositeFieldsContext
{
    public GroupWriterContext[] contexts;

    public CompositeFieldsContext(int[] redundantFieldIndexes)
    {
        contexts = ParseConfig();
        RecalculateIndexes(redundantFieldIndexes);
    }

    private GroupWriterContext[] ParseConfig()
    {
        XmlSerializer ser = new XmlSerializer(typeof(GroupWriterContext[]));

        StreamReader reader = new StreamReader($"{AppContext.BaseDirectory}/Configuration/GroupWriterConfig.xml");

        var toRet = ser.Deserialize(reader) as GroupWriterContext[]
            ?? throw new NullReferenceException("Cannot deserialize discretionary writer configuration.");

        return toRet;
    }

    private void RecalculateIndexes(int[] redundantFields)
    {
        List<GroupWriterContext> contextsList = contexts.ToList();

        for (int i = 0; i < contexts.Length; i++)
        {
            if (redundantFields.Contains(contexts[i].StartingIndex))
            {
                contextsList.Remove(contexts[i]);
            }
            contexts[i].StartingIndex -= redundantFields.Where(f => f < contexts[i].StartingIndex).Count();
        }

        contexts = contextsList.ToArray();
    }
}
