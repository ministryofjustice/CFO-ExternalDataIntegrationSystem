using Offloc.Cleaner.Cleaners;
using Serilog;

namespace Offloc.Cleaner.Services;

public abstract class CleaningStrategyBase(int[] redundantFieldIndexes)
{
    public void ProcessFile(string file)
    {
        FileCleaner fc = new FileCleaner(file, redundantFieldIndexes);
        // We are here. Lets try to clean the line 
        var clean = fc.Clean();
        // delete the original
        File.Delete(file);
        // move the clean one over the old name
        File.Move(clean, file);

        Log.Information($"File '{file}' has been cleaned, and is now ready for parsing");
    }
}