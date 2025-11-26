namespace Infrastructure.Entities.Offloc;

public partial class Location
{
    public string NomsNumber { get; set; } = null!;

    public string? Location1 { get; set; }

    public bool IsActive { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual PersonalDetail PersonalDetail { get; set; } = null!;
    #endregion
}
