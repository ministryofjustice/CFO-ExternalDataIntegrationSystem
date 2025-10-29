namespace Infrastructure.Entities.Delius;

public partial class OffenderManager
{
    public string OmCode { get; set; } = null!;

    public string? OmForename { get; set; }

    public string? OmSurname { get; set; }

    public string? OrgCode { get; set; }

    public string TeamCode { get; set; } = null!;

    public string? ContactNo { get; set; }
}
