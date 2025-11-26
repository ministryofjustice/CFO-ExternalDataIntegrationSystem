namespace Infrastructure.Entities.Delius;

public partial class OffenderManagerBuilding
{
    public byte[] CompositeHash { get; set; } = null!;

    public string? BuildingName { get; set; }

    public string? PostCode { get; set; }

    public string? HouseNumber { get; set; }

    public string? Street { get; set; }

    public string? District { get; set; }

    public string? Town { get; set; }

    public string? County { get; set; }
}
