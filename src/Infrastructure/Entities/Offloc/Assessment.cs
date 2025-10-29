namespace Infrastructure.Entities.Offloc;

public partial class Assessment
{
    public string NomsNumber { get; set; } = null!;

    public string? SecurityCategory { get; set; }

    public DateOnly? DateSecurityCategoryReview { get; set; }

    public DateOnly? DateSecCatChanged { get; set; }

    public bool IsActive { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual PersonalDetail PersonalDetail { get; set; } = null!;
    #endregion
}
