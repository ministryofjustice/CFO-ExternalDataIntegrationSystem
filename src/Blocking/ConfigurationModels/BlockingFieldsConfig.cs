namespace Blocking.ConfigurationModels;

//For making configuration type-safe.
public class BlockingFieldsConfig
{
    //Blocking fields in the same inner array will be blocked on together (they will all need to match to be considered a candidate).
    public BlockingFieldsGroup[] BlockingFieldsGroups { get; init; } = [];
}
