
namespace FileStorage;

public class AWSService : IFileService
{

    public AWSService()
    {
        
    }

    public Task<Stream> GetOfflocFile()
    {
        throw new NotImplementedException();
    }

    public Task<string> GetOfflocFileNames()
    {
        throw new NotImplementedException();
    }
}
