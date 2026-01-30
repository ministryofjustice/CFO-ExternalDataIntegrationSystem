namespace Infrastructure.Entities.Offloc;

public partial class ProcessedFile
{
    public string FileName { get; set; } = null!;

    public int FileId { get; set; }

    public DateTime ValidFrom {get;set;}

    public string Status{ get;set;} = null!;
}
