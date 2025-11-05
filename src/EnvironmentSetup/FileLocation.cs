
namespace FileStorage;


public interface IFileLocations
{
    string basePath { get;}
    string deliusInput { get; }
    string deliusOutput { get; }
    string offlocInput { get; }
    string offlocOutput { get; }
    string keyLocations { get; }
}

//TO-DO Put these classes in configurations in future.
//File paths for access from containers
public class DockerFileLocations : IFileLocations
{
    public DockerFileLocations()
    {
        this._basePath = "/app/";
    }
    private string _basePath;
    public string basePath { get => _basePath; }

    public string deliusInput => $"{basePath}delius/input";
    public string deliusOutput => $"{basePath}delius/output";

    //The offloc cleaner and offloc parser use the same input name.
    public  string offlocInput => $"{basePath}offloc/input"; 
    public  string offlocOutput => $"{basePath}offloc/output";
    public string keyLocations => $"{basePath}keys";


}

//Assumes dev work is always done on windows.
public class LocalFileLocations : IFileLocations
{
    //Default.
    //public new string basePath = $"C:/Users/{System.Security.Principal.WindowsIdentity.GetCurrent().Name!.Split("\\").Last().ToLower()}/DMS/";
    private string _basePath;
    public LocalFileLocations(string basePath)
    {
        this._basePath = basePath.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    }
    public string basePath { get => _basePath; }

    public string deliusInput => $"{basePath}Delius/Input";
    public string deliusOutput => $"{basePath}Delius/Output";

    //The offloc cleaner and offloc parser use the same input name.
    public string offlocInput => $"{basePath}Offloc/Input";
    public string offlocOutput => $"{basePath}Offloc/Output";
    public string keyLocations => "~/AppData/Local/ASP.NET/DataProtection-Keys";


}