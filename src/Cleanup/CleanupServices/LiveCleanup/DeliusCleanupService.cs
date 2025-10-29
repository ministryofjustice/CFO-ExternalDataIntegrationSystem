
using FileStorage;

namespace Cleanup.CleanupServices.LiveCleanup;

public class DeliusCleanupService : CleanupServiceBase
{
    public DeliusCleanupService(IFileLocations fileLocations)
        : base(fileLocations.deliusInput, fileLocations.deliusOutput)
    { }

	public override void ClearIllegalFiles()
	{
		DirectoryInfo di = new DirectoryInfo(outputFolderPath);
		var outputDirs = di.GetDirectories();

		foreach (var dir in outputDirs)
		{
            var illegalFiles = dir.GetFiles("*_new*");

            foreach (var file in illegalFiles)
            {
                File.Delete(file.FullName);
            }
        }
	}
}
