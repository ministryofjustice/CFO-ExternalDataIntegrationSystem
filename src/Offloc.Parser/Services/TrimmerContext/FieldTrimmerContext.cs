
using FileStorage;
using Microsoft.Extensions.Configuration;
using Offloc.Parser.Configuration;
using Offloc.Parser.Services.TrimmerContext.CoreContexts;
using Offloc.Parser.Services.TrimmerContext.SecondaryContexts;

namespace Offloc.Parser.Services.TrimmerContext;

public class FieldTrimmerContext
{
    public int[] redundantFields = { };

    public int[] compositeFieldIndexes => 
        compositeFieldsContext.contexts.Select(s => s.StartingIndex).ToArray();

    public CompositeFieldsContext compositeFieldsContext;
    public DiscretionaryFieldsContext discretionaryFieldsContext;

    //Duplicated in previous contexts but necessary as they will be treated
    //differently in writers.
    public DateTimeFieldContext datetimeFieldsContext;
    public AddressFieldsContext addressFieldsContext;

    //Bit naff.
    public FieldTrimmerContext(IConfiguration config)
    {
        string commaString = config.GetValue<string>("RedundantOfflocFields")!;

        string[] fields = commaString.Split(',');
        int[] castFields = new int[fields.Length];
        for (int i = 0; i < fields.Length; i++)
        {
            castFields[i] = OfflocNameIndexMappings.NameIndexDictionary[fields[i].Trim().ToLower()];
        }
        redundantFields = castFields;

        datetimeFieldsContext = new DateTimeFieldContext(redundantFields);
        compositeFieldsContext = new CompositeFieldsContext(redundantFields);
        discretionaryFieldsContext = new DiscretionaryFieldsContext(redundantFields);
        addressFieldsContext = new AddressFieldsContext(redundantFields);
    }
}
