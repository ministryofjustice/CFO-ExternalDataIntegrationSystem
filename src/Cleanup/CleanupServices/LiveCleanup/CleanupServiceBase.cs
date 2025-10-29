
using System.Globalization;

namespace Cleanup.CleanupServices.LiveCleanup;

public abstract class CleanupServiceBase
{
    protected CultureInfo cultureInfo = new CultureInfo("en-UK");

    protected string inputFolderPath;
    protected string outputFolderPath;

    public CleanupServiceBase(string inputFolderPath, string outputFolderPath)
    {
        this.inputFolderPath = inputFolderPath;
        this.outputFolderPath = outputFolderPath;
    }

    public void Cleanup(string fileName)
    {
        DeleteOutputFolder(fileName);
        DeleteInputFile(fileName);
    }

    protected void DeleteInputFile(string fileName)
    {
        string path = $"{inputFolderPath}/{fileName}";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    protected void DeleteOutputFolder(string fileName)
    {
        string folderName = fileName.Split('.').First();
        string path = $"{outputFolderPath}/{folderName}";
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public abstract void ClearIllegalFiles();

}
