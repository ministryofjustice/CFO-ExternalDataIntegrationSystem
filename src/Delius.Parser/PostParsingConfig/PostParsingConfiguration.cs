
namespace Delius.Parser.PostParsingConfig;

//This splits up some of the data from the generated files further (as these are imposed on us from how the
//input file is written). 
public class PostParsingConfiguration
{
    public string BaseFileName { get; set; } = string.Empty;
    public string NewFileName { get; set; } = string.Empty;
    public int[] RelevantFields { get; set; } = Array.Empty<int>();
    public int[] PrimaryKeyIndexes { get; set; } = Array.Empty<int>();
    public int[] ForeignKeyFieldIndexes { get; set; } = Array.Empty<int>(); //Defines which fields will be left in both files. 
    public int[] FieldsToNormalize { get; set; } = Array.Empty<int>(); //Defines which fields will be left in both files. 
    public bool GenerateCompositeHash { get; set; } = false;
    //Should be exactly equal to the NewFileName of another configuration.
    public string HashSourceTable { get; set; } = string.Empty;
}
