
using Delius.Parser.Core;
using FileStorage;
using Messaging.Interfaces;

namespace Delius.Parser.Services;

public class ParallelParsingStrategy : ParsingStrategyBase, IParsingStrategy
{
    public ParallelParsingStrategy(IFileProcessor fileProcessor, IStatusMessagingService statusService, 
        IStagingMessagingService stagingService, IFileLocations fileLocations) 
        : base(fileProcessor, statusService, stagingService, fileLocations) { }

    public async Task ParseFiles(string[] files)
    {
        await Task.CompletedTask;

        Parallel.For(0, files.Length, async (i, f) =>
        {
            await ParseFile(files[i]);
        });
    }
}
