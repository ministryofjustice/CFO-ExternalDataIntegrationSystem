
namespace Delius.Parser.Services;

public interface IParsingStrategy
{
    Task ParseFileAsync(string file);
}
