namespace Infrastructure.Entities.Offloc;

public partial class IncentiveLevel
{
    public string NomsNumber { get; set; } = null!;

    public string? IncentiveBand { get; set; }

    public bool IsActive { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual PersonalDetail PersonalDetail { get; set; } = null!;
    #endregion
}
