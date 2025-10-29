
namespace Offloc.Cleaner.Services;

public interface ICleaningStrategy
{
    Task CleanFiles(string[] files);
}
