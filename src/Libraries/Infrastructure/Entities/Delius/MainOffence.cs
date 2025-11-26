namespace Infrastructure.Entities.Delius;

public partial class MainOffence
{
    public long OffenderId { get; set; }

    public long Id { get; set; }

    public long EventId { get; set; }

    public string? OffenceCode { get; set; }

    public string? OffenceDescription { get; set; }

    public DateOnly? OffenceDate { get; set; }

    public string? Deleted { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual Offender Offender { get; set; } = null!;
    #endregion
}
