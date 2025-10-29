namespace Offloc.Parser.Writers.GroupWriters;

//Group writer uses a starting index instead of an array of field indexes.
public abstract class GroupWriterBase : WriterBase, IWriter
{
    public int StartingIndex;
    protected bool removeDuplicates;

    public GroupWriterBase(string path, int StartingIndex, bool removeDuplicates)
        : base(path)
    {
        this.StartingIndex = StartingIndex;
        this.removeDuplicates = removeDuplicates;
    }

    public abstract Task WriteAsync(string NOMSNumber, string[] contents);

    //This code checks for duplicates and removes them for select writers
    //(defined in ignore duplicates config file).
    protected string[] RemoveDuplicates(string[] splitString)
    {
        List<string> pastFields = new List<string>(splitString.Length/2);
        
        for (int i = 0; i < splitString.Length; i++)
        {
            if (!pastFields.Contains(splitString[i]))
            {
                pastFields.Add(splitString[i]);
            }
        }
        
        return pastFields.ToArray();
    }
}