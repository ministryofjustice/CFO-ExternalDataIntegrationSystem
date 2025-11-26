namespace Infrastructure.Entities.Delius;

public partial class OffenderManagerTeam
{
    public string OrgCode { get; set; } = null!;

    public string? OrgDescription { get; set; }

    public string TeamCode { get; set; } = null!;

    public string? TeamDescription { get; set; }

    public byte[] CompositeBuildingHash { get; set; } = null!;
}
