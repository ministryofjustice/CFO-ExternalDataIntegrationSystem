
namespace Offloc.Cleaner.Services;

public interface ICleaningStrategy
{
    Task CleanFile(string file);
}
