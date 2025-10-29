using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StagingMessages;
using Offloc.Parser.Services.TrimmerContext;

namespace Offloc.Parser.Services;

public class SequentialParsingStrategy : ParsingStrategyBase, IParsingStrategy
{
    private static SemaphoreSlim sem = new SemaphoreSlim(1, 1);

    public SequentialParsingStrategy(IStagingMessagingService stagingService, 
        IStatusMessagingService statusService, IFileLocations fileLocations, 
        FieldTrimmerContext trimmerContext)
        : base(stagingService, statusService, fileLocations, trimmerContext) { }

    public async Task ParseFiles(string[] files)
    {
        if (files.Length != 1)
        {
            throw new ArgumentException($"Files should be an array of length 1 here, not {files.Length}.");
        }
        else
        {
            await sem.WaitAsync();

            await ParseFile(fileLocations.offlocInput + '/' + files[0]);
            stagingService.StagingPublish(new OfflocParserFinishedMessage(files[0].Split('/').Last(), false));
            
            sem.Release();
        }
    }
}
