namespace Infrastructure.Entities.Delius;

public partial class ProcessedFile
{
    public string FileName { get; set; } = null!;

    public int FileId { get; set; }
}
