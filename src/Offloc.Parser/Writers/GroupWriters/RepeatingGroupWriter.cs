namespace Offloc.Parser.Writers.GroupWriters;

public class RepeatingGroupWriter : GroupWriterBase
{
    public RepeatingGroupWriter(string path, int fieldIndex, bool ignoreDuplicates)
        : base(path, fieldIndex, ignoreDuplicates) { }

    public override async Task WriteAsync(string NOMSNumber, string[] contents)
    {
        if (StreamWriter == null)
        {
            // this should never happen as the StreamWriter is created in the base constructor.
            throw new ApplicationException("Attempt to call WriteAsync with a null StreamWriter");
        }

        string content = contents[StartingIndex];

        if (string.IsNullOrWhiteSpace(content) == false)
        {
            string[] split = content.Split(new[] { "\"~\"" }, StringSplitOptions.RemoveEmptyEntries);

            if (removeDuplicates)
            {
                split = RemoveDuplicates(split);
            }

            foreach (var item in split)
            {
                await StreamWriter.WriteAsync($"{NOMSNumber}|{item}");
                await StreamWriter.WriteLineAsync();
            }
        }
    }
}