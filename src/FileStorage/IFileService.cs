
namespace FileStorage;

public interface IFileService
{
    Task<string> GetOfflocFileNames();
    Task<Stream> GetOfflocFile();

}
