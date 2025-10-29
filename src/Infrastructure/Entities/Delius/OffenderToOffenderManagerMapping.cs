namespace Infrastructure.Entities.Delius;

public partial class OffenderToOffenderManagerMapping
{
    public long OffenderId { get; set; }

    public long Id { get; set; }

    public DateOnly? AllocatedDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string OmCode { get; set; } = null!;
    public string OrgCode { get; set; } = null!;
    public string TeamCode { get; set; } = null!;

    public string? Deleted { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual Offender Offender { get; set; } = null!;
    #endregion
}
