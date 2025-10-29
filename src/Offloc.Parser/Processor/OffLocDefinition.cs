using Offloc.Parser.Services.TrimmerContext;
using Offloc.Parser.Writers;
using Offloc.Parser.Writers.Factory;
using System.Text.RegularExpressions;

namespace Offloc.Parser.Processor;

internal class OffLocDefinition : IDisposable
{
    private readonly List<IWriter> writers;
    private int expectedNoOfFields;

    public OffLocDefinition(WriterFactory factory, FieldTrimmerContext trimmerContext)
    {    
        writers = factory.CreateWriters().ToList();
        expectedNoOfFields = (153-trimmerContext.redundantFields.Length);
    }

    public async Task Handle(string line, string NOMSNumber)
    {   
        if (line != string.Empty)
        {
            line = Regex.Replace(line, @"\s+", " "); //Should replace double spaces with single.
            
            string[] split = line.Split(new[] { "\"|\"" }, StringSplitOptions.None)
                .Select(s => s.Trim()) //Attempt to remove excess whitespace from columns. 
                .ToArray();
                        
            if (VerifySplit(split))
            {
                foreach (var item in writers)
                {
                    await item.WriteAsync(NOMSNumber, split);
                }
            }            
        }
    }

    private bool VerifySplit(string[] split)
    {
        return split.Length == expectedNoOfFields;
    }

    public void Dispose()
    {
        writers.ForEach(w => w.Dispose());
    }
}