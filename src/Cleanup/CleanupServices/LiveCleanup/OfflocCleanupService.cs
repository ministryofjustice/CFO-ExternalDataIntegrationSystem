using FileStorage;

namespace Cleanup.CleanupServices.LiveCleanup;

public class OfflocCleanupService(IFileLocations fileLocations) : CleanupServiceBase(fileLocations.offlocInput, fileLocations.offlocOutput)
{
    //A method to make DMS more resilient (by stopping it from trying to process half-cleaned files).
    public override void ClearIllegalFiles()
	{
		DirectoryInfo di = new DirectoryInfo(inputFolderPath);
		var illegalFiles = di.GetFiles("*_clean*");

		foreach (var illegalFile in illegalFiles) 
		{ 
			File.Delete(illegalFile.FullName);
		}
	}
}
