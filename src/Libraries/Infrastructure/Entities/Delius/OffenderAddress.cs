namespace Infrastructure.Entities.Delius;

public partial class OffenderAddress
{
    public long OffenderId { get; set; }

    public long Id { get; set; }

    public string? StatusCode { get; set; }

    public string? StatusDescription { get; set; }

    public string? BuildingName { get; set; }

    public string? HouseNumber { get; set; }

    public string? StreetName { get; set; }

    public string? District { get; set; }

    public string? Town { get; set; }

    public string? County { get; set; }

    public string? Postcode { get; set; }

    public DateOnly? StartDate { get; set; }

    public string? NoFixedAbode { get; set; }

    public string? Deleted { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual Offender Offender { get; set; } = null!;
    #endregion
}
