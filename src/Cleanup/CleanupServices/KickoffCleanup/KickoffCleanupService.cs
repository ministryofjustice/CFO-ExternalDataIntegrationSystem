using FileStorage;

namespace Cleanup.CleanupServices.KickoffCleanup;

//Clears half cleaned files and staging database on kickoff.
public class KickoffCleanupService
{
    private readonly IFileLocations fileLocations;

    public KickoffCleanupService(IFileLocations fileLocations)
    {
        this.fileLocations = fileLocations;
    }

    //Deletes half cleaned files from offloc input directory and output directories.
    public void TidyOfflocInputFolder()
    {
        string[] filePaths = Directory.GetFiles(fileLocations.offlocInput);

        for (int i = 0; i < filePaths.Length; i++)
        {
            if (filePaths[i].Split("/").Contains("clean") ||
                filePaths[i].Split("\\").Contains("clean"))
            {
                File.Delete(filePaths[i]);
            }
        }
    }
}
