
using Offloc.Parser.Configuration;
using System.Xml.Serialization;

namespace Offloc.Parser.Services.TrimmerContext;

public class DiscretionaryWriterContext
{
    public string TableName { get; init; } = string.Empty;
    [XmlIgnore]
    public int[] RelevantFields { get; set; } = { };
    public string[] StringRelevantFields { get; init; } = { };
    public bool IncludeId { get; init; } = true;

    //Could also try registering nameIndexMappings as a dependency and doing this
    //in a constructor to reduce dependency.
    public void MapToIndexes()
    {
        List<int> relevantFields = new();
        foreach (var field in StringRelevantFields)
        {
            string normalizedField = field.ToLower();
            //Not as efficient as 2 lookups need but TryGetValue not working as expected.
            int value;
            if (OfflocNameIndexMappings.NameIndexDictionary.TryGetValue(normalizedField, out value))
            {
                relevantFields.Add(value);
            }
        }

        RelevantFields = relevantFields.ToArray();
     }
}
