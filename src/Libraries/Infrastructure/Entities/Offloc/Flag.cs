namespace Infrastructure.Entities.Offloc;

public partial class Flag
{
    public string NomsNumber { get; set; } = null!;

    public string Details { get; set; } = null!;

    public bool IsActive { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual PersonalDetail PersonalDetail { get; set; } = null!;
    #endregion
}
