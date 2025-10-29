namespace Infrastructure.Entities.Offloc;

public partial class Booking
{
    public string NomsNumber { get; set; } = null!;

    public string PrisonNumber { get; set; } = null!;

    public DateOnly FirstReceptionDate { get; set; }

    public bool IsActive { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual PersonalDetail PersonalDetail { get; set; } = null!;
    #endregion
}
