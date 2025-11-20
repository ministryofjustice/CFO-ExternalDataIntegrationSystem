
using System.Diagnostics;
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

    public Task ParseFileAsync(string file) => ProcessFile(file);
}
