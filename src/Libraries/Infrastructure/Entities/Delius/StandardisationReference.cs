namespace Infrastructure.Entities.Delius;

public partial class StandardisationReference
{
    public string? RawData { get; set; }

    public string? CleanedData { get; set; }

    public string? Source { get; set; }

    public string? Type { get; set; }
}
