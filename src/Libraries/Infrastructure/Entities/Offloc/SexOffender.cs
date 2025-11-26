namespace Infrastructure.Entities.Offloc;

public partial class SexOffender
{
    public string NomsNumber { get; set; } = null!;

    public string? Schedule1Sexoffender { get; set; }

    public bool IsActive { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual PersonalDetail PersonalDetail { get; set; } = null!;
    #endregion
}
