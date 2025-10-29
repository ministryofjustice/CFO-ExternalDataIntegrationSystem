namespace Infrastructure.Entities.Offloc;

public partial class OffenderStatus
{
    public string NomsNumber { get; set; } = null!;

    public string? CustodyStatus { get; set; }

    public string? InmateStatus { get; set; }

    public bool IsActive { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual PersonalDetail PersonalDetail { get; set; } = null!;
    #endregion
}
