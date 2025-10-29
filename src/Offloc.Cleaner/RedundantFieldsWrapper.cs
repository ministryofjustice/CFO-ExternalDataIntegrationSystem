
using Offloc.Cleaner.Configuration;

namespace Offloc.Cleaner;

public class RedundantFieldsWrapper
{
    public int[] redundantFieldIndexes { get; set; } = { };

    public RedundantFieldsWrapper()
    { }
    //Bit naff.
    public RedundantFieldsWrapper(string commaString)
    {
        string[] fields = commaString.Split(',');
        int[] castFields = new int[fields.Length];
        for (int i = 0; i < fields.Length; i++)
        {
            castFields[i] = OfflocNameIndexMappings.NameIndexDictionary[fields[i].Trim().ToLower()];
        }
        redundantFieldIndexes = castFields;
    }
}
