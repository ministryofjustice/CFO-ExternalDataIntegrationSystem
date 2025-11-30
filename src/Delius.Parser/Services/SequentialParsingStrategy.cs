using Delius.Parser.Core;
using FileStorage;
using Messaging.Interfaces;

namespace Delius.Parser.Services;

public class SequentialParsingStrategy(
    IFileProcessor fileProcessor,
    IMessageService stagingMessagingService, 
    IFileLocations fileLocations) : ParsingStrategyBase(fileProcessor, stagingMessagingService, fileLocations), IParsingStrategy
{
    public Task ParseFileAsync(string file) => ProcessFileAsync(file);
}
