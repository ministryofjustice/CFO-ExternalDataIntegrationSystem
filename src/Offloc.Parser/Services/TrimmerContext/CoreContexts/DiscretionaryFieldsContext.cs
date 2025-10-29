
using FileStorage;
using Offloc.Parser.Configuration;
using System.Xml.Serialization;

namespace Offloc.Parser.Services.TrimmerContext.CoreContexts;

public class DiscretionaryFieldsContext
{
    //Used to create DiscretionaryWriters at runtime- can't be created yet as paths are unknown.
    public DiscretionaryWriterContext[] contexts = { };
    //private OfflocNameIndexMappings nameIndexMappings;

    public DiscretionaryFieldsContext(int[] redundantFields)
    {
        //nameIndexMappings = new OfflocNameIndexMappings(redundantFields);
        contexts = ParseConfig();
        RecalculateIndexes(redundantFields);
    }

    private DiscretionaryWriterContext[] ParseConfig()
    {
        XmlSerializer ser = new XmlSerializer(typeof(DiscretionaryWriterContext[]));

        StreamReader reader = new StreamReader($"{AppContext.BaseDirectory}/Configuration/DiscretionaryWriterConfig.xml");

        DiscretionaryWriterContext[] contexts = ser.Deserialize(reader) as DiscretionaryWriterContext[]
            ?? throw new NullReferenceException("Cannot deserialize discretionary writer configuration.");

        foreach (var context in contexts)
        {
            context.MapToIndexes();
        }

        return contexts;
    }

    private void RecalculateIndexes(int[] redundantFields)
    {
        foreach (var context in contexts)
        {
            for (int i = 0; i < context.RelevantFields.Length; i++)
            {
                context.RelevantFields[i] -= redundantFields.Where(f => f < context.RelevantFields[i]).Count();
            }
        }
    }
}
