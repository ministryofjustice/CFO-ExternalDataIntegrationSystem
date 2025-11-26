namespace Infrastructure.Entities.Delius;

public partial class PersonalCircumstance
{
    public long OffenderId { get; set; }

    public long Id { get; set; }

    public string? Type { get; set; }

    public string? TypeDescription { get; set; }

    public string? SubType { get; set; }

    public string? SubTypeDescription { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Deleted { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual Offender Offender { get; set; } = null!;
    #endregion
}
