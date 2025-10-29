
using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StatusMessages;
using System.Globalization;

namespace Offloc.Cleaner.Services;

public class SequentialCleaningStrategy : CleaningStrategyBase, ICleaningStrategy
{    public SequentialCleaningStrategy(IStagingMessagingService stagingService, 
        IStatusMessagingService statusService, IFileLocations fileLocations, 
        RedundantFieldsWrapper redundantWrapper) 
        : base(stagingService, statusService, fileLocations, 
            redundantWrapper.redundantFieldIndexes){ }
    
    public async Task CleanFiles(string[] files)
    {
        files = OrderFilesByDateAsc(files);

        for (int i = 0; i < files.Length; i++)
        { 
            statusService.StatusPublish(new StatusUpdateMessage($"Cleaning file: {files[i]}"));
            await CleanFile($"{fileLocations.offlocInput}/{files[i]}");
            stagingService.StagingPublish(new OfflocCleanerFinishedMessage(new string[] { files[i] }, redundantFieldIndexes));
        }
    }

    //Ordering of files is only needed if sequential processing is toggled.
    private string[] OrderFilesByDateAsc(string[] files)
    {
        Dictionary<string, DateTime> dictionary = new Dictionary<string, DateTime>();

        foreach (var file in files)
        {
            string s = file.Substring(17, 8);
            s = $"{s[..2]}/{s[2..4]}/{s[4..]}";
            DateTime datetime;
            DateTime.TryParseExact(s, new[] { "dd/MM/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out datetime);
            dictionary.Add(file, datetime);
        }
        dictionary = dictionary.OrderBy(d => d.Value).ToDictionary();

        return dictionary.Keys.ToArray();
    }
}
