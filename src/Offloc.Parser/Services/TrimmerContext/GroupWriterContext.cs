using Offloc.Parser.Services.TrimmerContext.CoreContexts.Enums;

namespace Offloc.Parser.Services.TrimmerContext;

public class GroupWriterContext
{
    public string TableName { get; init; } = string.Empty;
    public int StartingIndex { get; set; }
    public TGroupWriter WriterType { get; init; }
    public bool IgnoreDuplicates { get; init; }

    //Necessary for XML deserialization.
    public GroupWriterContext()
    { }

    public GroupWriterContext(int StartingIndex, string TableName)
    {
        this.StartingIndex = StartingIndex;
        this.TableName = TableName;
    }
}
