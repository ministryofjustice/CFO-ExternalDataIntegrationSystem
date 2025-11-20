
using System.Runtime.CompilerServices;

namespace DbInteractions.Services;

//This will likely need to be split up as the number of db interactions increases. 
public interface IDbInteractionService
{
    Task<string[]> GetProcessedDeliusFileNames();
    Task<int> DeliusGetFileIdLastFull();
    Task<string[]> GetProcessedOfflocFileNames();
    Task<int[]> GetProcessedOfflocIds();
    Task StageDelius(string fileName, string filePath);
    Task StageOffloc(string fileName);
    Task MergeDeliusPicture(string fileName);
    Task MergeOfflocPicture(string fileName);
    Task StandardiseDeliusStaging();
    Task ClearDeliusStaging();
    Task ClearOfflocStaging();
    Task CreateOfflocProcessedFileEntry(string fileName, int fileId, string? archiveName = null);
    Task CreateDeliusProcessedFileEntry(string fileName, string fileId);
    Task AssociateOfflocFileWithArchive(string fileName, string archiveName);
    Task<bool> IsDeliusReadyForProcessing();
    Task<bool> IsOfflocReadyForProcessing();
}
