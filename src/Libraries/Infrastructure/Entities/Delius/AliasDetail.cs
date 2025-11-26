namespace Infrastructure.Entities.Delius;

public partial class AliasDetail
{
    public int RowNo { get; set; }

    public long OffenderId { get; set; }

    public long Id { get; set; }

    public string? FirstName { get; set; }

    public string? SecondName { get; set; }

    public string? ThirdName { get; set; }

    public string? Surname { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? GenderCode { get; set; }

    public string? GenderDescription { get; set; }

    public string? Deleted { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual Offender Offender { get; set; } = null!;
    #endregion
}
