
namespace FileStorage;


public interface IFileLocations
{
    string basePath { get;}
    string deliusInput { get; }
    string deliusOutput { get; }
    string offlocInput { get; }
    string offlocOutput { get; }
}

public class FileLocations : IFileLocations
{
    private string _basePath;

    public FileLocations(string basePath)
    {
        this._basePath = basePath.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    }
    
    public string basePath { get => _basePath; }

    public string deliusInput => Path.Combine(basePath, "Delius", "Input");
    public string deliusOutput => Path.Combine(basePath, "Delius", "Output");

    //The offloc cleaner and offloc parser use the same input name.
    public string offlocInput => Path.Combine(basePath, "Offloc", "Input");
    public string offlocOutput => Path.Combine(basePath, "Offloc", "Output");
}