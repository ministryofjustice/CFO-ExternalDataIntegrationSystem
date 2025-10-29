
namespace Delius.Parser.Services;

public interface IParsingStrategy
{
    Task ParseFiles(string[] files);
}
