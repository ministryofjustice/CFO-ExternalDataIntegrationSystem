namespace Infrastructure.Entities.Delius;

public partial class Oa
{
    public long OffenderId { get; set; }

    public long Id { get; set; }

    public string? Roshscore { get; set; }

    public DateOnly? AssesmentDate { get; set; }

    public string? Deleted { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual Offender Offender { get; set; } = null!;
    #endregion
}
