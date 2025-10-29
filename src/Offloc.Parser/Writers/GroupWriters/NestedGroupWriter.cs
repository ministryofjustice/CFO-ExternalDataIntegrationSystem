namespace Offloc.Parser.Writers.GroupWriters;

public class NestedGroupWriter : GroupWriterBase
{
    public NestedGroupWriter(string path, int fieldIndex, bool removeDuplicates)
        : base(path, fieldIndex, removeDuplicates) { }

    public override async Task WriteAsync(string NOMSNumber, string[] contents)
    {
        if (StreamWriter == null)
        {
            // this should never happen as the StreamWriter is created in the base constructor.
            throw new ApplicationException("Attempt to call WriteAsync with a null StreamWriter");
        }

        string value = contents[StartingIndex];
        if (string.IsNullOrWhiteSpace(value) == false)
        {
            var temp = value.Split(new[] { '|' });
            if (temp.Length != 1)
            {
                value = temp.First().Remove(temp.First().Length - 1, 1);
            }
            string[] rows = value.Split(new[] { "\"~\"" }, StringSplitOptions.None);
            if (removeDuplicates)
            {
                rows = RemoveDuplicates(rows);
            }
            foreach (var row in rows)
            {
                string[] columns = row.Split(new[] { "\",\"" }, StringSplitOptions.None);
                if (columns.Length == 6) //To fix erroneous activty file output.
                {
                    //this is the offender id write
                    await StreamWriter.WriteAsync(NOMSNumber);
                    
                    foreach (var column in columns)
                    {
                        await StreamWriter.WriteAsync($"|{column}");
                    }
                    await StreamWriter.WriteLineAsync();
                }
            }
        }
    }
}
