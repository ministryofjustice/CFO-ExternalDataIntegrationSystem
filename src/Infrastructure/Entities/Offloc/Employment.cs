namespace Infrastructure.Entities.Offloc;

public partial class Employment
{
    public string NomsNumber { get; set; } = null!;

    public string? Employed { get; set; }

    public string? EmploymentStatusReception { get; set; }

    public string? EmploymentStatusDischarge { get; set; }

    public bool IsActive { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual PersonalDetail PersonalDetail { get; set; } = null!;
    #endregion
}
