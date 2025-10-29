
using Offloc.Parser.Services.TrimmerContext;
using Offloc.Parser.Services.TrimmerContext.CoreContexts.Enums;
using Offloc.Parser.Writers.GroupWriters;
using Offloc.Parser.Writers.GroupWriters.Address;
using Offloc.Parser.Writers.DiscretionaryWriters;

namespace Offloc.Parser.Writers.Factory;

internal class WriterFactory
{
    private readonly string targetDirectory;
    private FieldTrimmerContext trimmerContext;

    public WriterFactory(string targetDirectory, FieldTrimmerContext trimmerContext)
    {
        this.trimmerContext = trimmerContext;

        if(Directory.Exists(targetDirectory) == false)
        {
            Directory.CreateDirectory(targetDirectory);
        }

        this.targetDirectory = targetDirectory;
    }

    public IEnumerable<IWriter> CreateWriters()
    {
        List<IWriter> writers = new List<IWriter>(InitializeCompositeWriters());
        writers.AddRange(InitializeDiscretionaryWriters());
        writers.AddRange(InitializeCustomWriters());

        return writers;
    }

    private IWriter[] InitializeCustomWriters()
    {
        return [
            new AddressWriter(targetDirectory, trimmerContext.addressFieldsContext),
            new AgenciesWriter(targetDirectory, trimmerContext.redundantFields)
        ];
    }

    //The 2 methods below use the FieldTrimmerContext to initialize the writers.
    private GroupWriterBase[] InitializeCompositeWriters()
    {
        int contextLength = trimmerContext.compositeFieldsContext.contexts.Length;
        GroupWriterBase[] writers = new GroupWriterBase[contextLength];

        for (int i = 0; i < contextLength; i++)
        {
            GroupWriterContext context = trimmerContext.compositeFieldsContext.contexts[i];

            if (context.WriterType == TGroupWriter.Repeating)
            {
                writers[i] = new RepeatingGroupWriter(
                    Path.Combine(targetDirectory, $"{context.TableName}.txt"), 
                    context.StartingIndex, context.IgnoreDuplicates
                );
            }
            else if(context.WriterType == TGroupWriter.Nested)
            {
                writers[i] = new NestedGroupWriter(
                    Path.Combine(targetDirectory, $"{context.TableName}.txt"), 
                    context.StartingIndex, context.IgnoreDuplicates
                );
            }
        }

        return writers;
    }

    private IEnumerable<DiscretionaryWriter> InitializeDiscretionaryWriters()
    {
        int contextLength = trimmerContext.discretionaryFieldsContext.contexts.Length;
        DiscretionaryWriter[] writers = new DiscretionaryWriter[contextLength];
        for (int i = 0; i < contextLength; i++)
        {
            var context = trimmerContext.discretionaryFieldsContext.contexts[i];
            writers[i] = new DiscretionaryWriter(Path.Combine(targetDirectory, $"{context.TableName}.txt"), context, trimmerContext.datetimeFieldsContext);
        }

        return writers;
    }
}