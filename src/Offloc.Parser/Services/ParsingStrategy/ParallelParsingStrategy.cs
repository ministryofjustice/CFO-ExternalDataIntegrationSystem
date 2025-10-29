using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StagingMessages;
using Offloc.Parser.Services.TrimmerContext;

namespace Offloc.Parser.Services;

public class ParallelParsingStrategy : ParsingStrategyBase, IParsingStrategy
{
    public ParallelParsingStrategy(IStagingMessagingService stagingService,
        IStatusMessagingService statusService, IFileLocations fileLocations,
        FieldTrimmerContext trimmerContext) 
        : base(stagingService, statusService, fileLocations, trimmerContext){ }

    public async Task ParseFiles(string[] files)
    {
        await Task.CompletedTask;

        Parallel.For(0, files.Length, async (i, f) =>
        {
            await ParseFile(fileLocations.offlocInput + '/' + files[i]);
            stagingService.StagingPublish(new OfflocParserFinishedMessage(files[i].Split('/').Last(), false));
        });
    }
}