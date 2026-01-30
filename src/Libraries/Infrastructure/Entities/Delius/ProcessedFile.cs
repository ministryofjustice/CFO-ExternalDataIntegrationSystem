using Microsoft.Identity.Client;

namespace Infrastructure.Entities.Delius;

public partial class ProcessedFile
{
    public string FileName { get; set; } = null!;

    public int FileId { get; set; }

    public string Status {get; set;} = null!;

    public DateTime ValidFrom { get;set; }
}
