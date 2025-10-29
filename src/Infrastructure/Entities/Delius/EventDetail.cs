namespace Infrastructure.Entities.Delius;

public partial class EventDetail
{
    public long OffenderId { get; set; }

    public long Id { get; set; }

    public DateOnly? ReferralDate { get; set; }

    public DateOnly? ConvictionDate { get; set; }

    public string? Cohort { get; set; }

    public string? Deleted { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual Offender Offender { get; set; } = null!;
    #endregion
}
