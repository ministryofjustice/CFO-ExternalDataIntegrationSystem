
using Delius.Parser.Core;
using FileStorage;
using Messaging.Interfaces;

namespace Delius.Parser.Services;

public class SequentialParsingStrategy : ParsingStrategyBase, IParsingStrategy
{
    public SequentialParsingStrategy(IFileProcessor fileProcessor, 
        IStagingMessagingService stagingMessagingService, IStatusMessagingService statusMessagingService, 
        IFileLocations fileLocations) 
        : base(fileProcessor, statusMessagingService, stagingMessagingService, fileLocations) { }

    public async Task ParseFiles(string[] files)
    {
        files = files.Order().ToArray();
        for(int i = 0; i<files.Length; i++)
        {
            await ParseFile(files[i]);
        }
    }
}
