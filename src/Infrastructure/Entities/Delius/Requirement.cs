namespace Infrastructure.Entities.Delius;

public partial class Requirement
{
    public long OffenderId { get; set; }

    public long Id { get; set; }

    public long DisposalId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? CommencementDate { get; set; }

    public DateOnly? Terminationdate { get; set; }

    public string? TerminationReasonCode { get; set; }

    public string? TerminationDescription { get; set; }

    public string? CategoryCode { get; set; }

    public string? CategoryDescription { get; set; }

    public string? SubCategoryCode { get; set; }

    public string? SubCategoryDescription { get; set; }

    public string? Length { get; set; }

    public string? UnitCode { get; set; }

    public string? UnitDescription { get; set; }

    public string? Deleted { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual Offender Offender { get; set; } = null!;
    #endregion
}
