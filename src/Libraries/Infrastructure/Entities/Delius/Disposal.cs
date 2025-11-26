namespace Infrastructure.Entities.Delius;

public partial class Disposal
{
    public long OffenderId { get; set; }

    public long Id { get; set; }

    public long? EventId { get; set; }

    public DateOnly? SentenceDate { get; set; }

    public string? Length { get; set; }

    public string? UnitCode { get; set; }

    public string? UnitDescription { get; set; }

    public string? DisposalCode { get; set; }

    public string? DisposalDetail { get; set; }

    public string? DisposalTerminationCode { get; set; }

    public string? DisposalTerminationDescription { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public string? Deleted { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual Offender Offender { get; set; } = null!;
    #endregion
}
