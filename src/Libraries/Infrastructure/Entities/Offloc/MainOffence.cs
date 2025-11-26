namespace Infrastructure.Entities.Offloc;

public partial class MainOffence
{
    public string NomsNumber { get; set; } = null!;

    public string? MainOffence1 { get; set; }

    public DateOnly? DateFirstConviction { get; set; }

    public bool IsActive { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual PersonalDetail PersonalDetail { get; set; } = null!;
    #endregion
}
