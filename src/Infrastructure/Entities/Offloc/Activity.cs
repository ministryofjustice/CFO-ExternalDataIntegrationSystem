namespace Infrastructure.Entities.Offloc;

public partial class Activity
{
    public string NomsNumber { get; set; } = null!;

    public string Activity1 { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int StartHour { get; set; }

    public int StartMin { get; set; }

    public int EndHour { get; set; }

    public int EndMin { get; set; }

    public bool IsActive { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual PersonalDetail PersonalDetail { get; set; } = null!;
    #endregion
}
