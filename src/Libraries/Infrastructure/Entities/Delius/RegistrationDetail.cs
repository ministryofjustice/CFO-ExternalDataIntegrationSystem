namespace Infrastructure.Entities.Delius;

public partial class RegistrationDetail
{
    public long OffenderId { get; set; }

    public long Id { get; set; }

    public DateOnly? Date { get; set; }

    public string? TypeCode { get; set; }

    public string? TypeDescription { get; set; }

    public string? CategoryCode { get; set; }

    public string? CategoryDescription { get; set; }

    public string? RegisterCode { get; set; }

    public string? RegisterDescription { get; set; }

    public string? DeRegistered { get; set; }

    public string? Deleted { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual Offender Offender { get; set; } = null!;
    #endregion
}
