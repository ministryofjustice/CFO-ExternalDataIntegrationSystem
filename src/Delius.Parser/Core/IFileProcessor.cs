namespace Delius.Parser.Core;

public interface IFileProcessor
{
    Task Process(string fileToBeParsed, string outputDirectory);
}
