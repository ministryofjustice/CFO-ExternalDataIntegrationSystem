using Offloc.Parser.Services.TrimmerContext;
using Offloc.Parser.Writers.Factory;
using Serilog;

namespace Offloc.Parser.Processor;

public class OfflocProcessor
{
    private string fileToProcess;
    private string outputDirectory;

    private FieldTrimmerContext trimmerContext;

    public OfflocProcessor(string fileToProcess, string outputDirectory, FieldTrimmerContext trimmerContext)
    {
        this.fileToProcess = fileToProcess;
        this.outputDirectory = outputDirectory;
        this.trimmerContext = trimmerContext;
    }

    public async Task Process()
    {
        var outputPath = outputDirectory.Substring(0, outputDirectory.LastIndexOf(".dat"));
        
        using var definition = new OffLocDefinition(new WriterFactory(outputPath, trimmerContext), trimmerContext);

        await using var fs = new FileStream(fileToProcess, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096);

        using StreamReader sr = new(fs);

        int cnt = 0;
        while (await sr.ReadLineAsync() is { } line)
        {
            string NOMSNo;

			try
            {
				NOMSNo = line.Split("\"|\"")[3];
			}
            catch(IndexOutOfRangeException ex)
            {
                Console.WriteLine($"Line {line} is not long enough to be parsed.");
                continue;
            }

			await definition.Handle(line, NOMSNo);
            cnt++;
        }

        Log.Information($"Finished reading file '{fileToProcess}'. Found {cnt} records");
    }
}

public struct Field
{
    public string Name { get; set; }
    public string FieldDeliminator { get; set; }
    public string RowDeliminator { get; set; }
}