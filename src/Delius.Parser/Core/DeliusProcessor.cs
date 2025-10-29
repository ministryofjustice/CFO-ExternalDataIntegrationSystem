using Delius.Parser.Configuration;
using Delius.Parser.Configuration.Models;
using Serilog;

namespace Delius.Parser.Core;

public class DeliusProcessor
{
    private Line[] Lines { get; }
    private readonly DeliusOutputter outputter;
    private PostParser postParser;
    private string outputPath = string.Empty;

    public DeliusProcessor(DeliusOutputter outputer, PostParser postParser)
    {
        Lines = ConfigurationParser.GetLines(); //Gets lines from configuration file.

        outputter = outputer;
        this.postParser = postParser;
    }

    public async Task Process(StreamReader reader, string outputPath, Action<string> unhandledLine) //
    {
        this.outputPath = outputPath;

        using (reader)
        {
            string? text = null;

            string? detailKey = null;

            while ((text = await reader.ReadLineAsync()) != null)
            {
                detailKey = await ProcessLine(unhandledLine, text, detailKey);
            }
            outputter.Finish();
            await postParser.PostParse(outputPath);
        }
    }

    private async Task<string> ProcessLine(Action<string> unhandledLine, string text, string? detailKey)
    {
        Line line = GetLineFor(text, detailKey)!;

        if (line == null)
        {
            unhandledLine(text);
        }
        else
        {
            if (line.StartingKey == "DS")
            {
                detailKey = line.Split(text)[0];
            }
            await Process(text, line);
        }
        return detailKey;
    }

    private async Task Process(string text, Line line)
    {
        string[] values = line.Split(text);

        if (line.Fields.Any(r => r.Name.Equals("OffenderId")))
        {
            Field f = line.Fields.First(n => n.Name.Equals("OffenderId"));
            long offenderId = long.Parse(f.Parse(text));
            outputter.SetOffenderId(offenderId);
        }

        if (line.OutputToFile)
        {
            outputter.StartOutput(line.Name, outputPath);
            foreach (var item in values)
            {
                await outputter.WriteAsync(item);
            }
            await outputter.EndLine();
        }
        if (line.OutputToLog)
        {
            Log.Information($"Found {line.Name}");
            foreach (var item in values)
            {
                Log.Information("\t " + item == string.Empty ? "Empty" : item);
            }
        }
    }

    private Line? GetLineFor(string text, string detailKey)
    {
        var lines = from l in Lines
                    where
                        l.Length == text.Length
                        && text.StartsWith(l.StartingKey)
                        && (detailKey == null || l.ParentKey == null || l.ParentKey == detailKey)
                    select l;

        return lines.SingleOrDefault();
    }
}
