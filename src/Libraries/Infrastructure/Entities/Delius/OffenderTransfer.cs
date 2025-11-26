namespace Infrastructure.Entities.Delius;

public partial class OffenderTransfer
{
    public long OffenderId { get; set; }

    public long Id { get; set; }

    public DateOnly? RequestDate { get; set; }

    public string? ReasonCode { get; set; }

    public string? ReasonDescription { get; set; }

    public string? StatusCode { get; set; }

    public string? StatusDescription { get; set; }

    public string? Deleted { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual Offender Offender { get; set; } = null!;
    #endregion
}
