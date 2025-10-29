using Offloc.Parser.Services.TrimmerContext;
using Offloc.Parser.Services.TrimmerContext.SecondaryContexts;

namespace Offloc.Parser.Writers.DiscretionaryWriters;

//Discretionary writer extracts individual fields into a table.
public class DiscretionaryWriter : WriterBase, IWriter
{
    public string tableName = string.Empty;
    public int[] relevantFields = { };
    private bool includeId;

    private readonly DateTimeFieldContext datetimeFieldContext;

    public DiscretionaryWriter(string path, DiscretionaryWriterContext context,
        DateTimeFieldContext datetimeFieldContext) : base(path)
    {
        tableName = context.TableName;
        relevantFields = context.RelevantFields;
        includeId = context.IncludeId;
        this.datetimeFieldContext = datetimeFieldContext;
    }

    public async Task WriteAsync(string NOMSNumber, string[] contents)
    {
        string[] subContents = contents.Where(s => relevantFields.Contains((Array.IndexOf(contents, s)))).ToArray();

        //Only outputs line if one of the fields isn't blank. This is generally 
        //common in single-column tables.

        if (subContents.Any(s => !string.IsNullOrEmpty(s)))
        {
            string toWrite = includeId ? $"{NOMSNumber}" : "";

            for (int i = 0; i < relevantFields.Length; i++)
            {
                toWrite = string.Join('|', new string[] { toWrite, contents[relevantFields[i]].Trim('\"') });
            }
            await StreamWriter!.WriteLineAsync(toWrite);
        }
    }
}
