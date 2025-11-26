namespace Infrastructure.Entities.Offloc;

public partial class Address
{
    public string NomsNumber { get; set; } = null!;

    public string AddressType { get; set; } = null!;

    public string? NominatedNok { get; set; }

    public string? AddressRelationship { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Address3 { get; set; }

    public string? Address4 { get; set; }

    public string? Address5 { get; set; }

    public string? Address6 { get; set; }

    public string? Address7 { get; set; }

    public bool IsActive { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual PersonalDetail PersonalDetail { get; set; } = null!;
    #endregion
}
