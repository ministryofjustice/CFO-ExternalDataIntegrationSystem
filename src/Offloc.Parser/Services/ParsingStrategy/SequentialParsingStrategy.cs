using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StagingMessages;
using Offloc.Parser.Services.TrimmerContext;

namespace Offloc.Parser.Services;

public class SequentialParsingStrategy(
    IMessageService messageService, 
    IFileLocations fileLocations,
    FieldTrimmerContext trimmerContext) : ParsingStrategyBase(messageService, fileLocations, trimmerContext), IParsingStrategy
{
    private static SemaphoreSlim sem = new SemaphoreSlim(1, 1);

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
            await messageService.PublishAsync(new OfflocParserFinishedMessage(files[0].Split('/').Last(), false));
            
            sem.Release();
        }
    }
}
