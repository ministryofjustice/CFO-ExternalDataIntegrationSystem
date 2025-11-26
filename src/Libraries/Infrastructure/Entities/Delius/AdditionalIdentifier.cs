using System.Text.Json.Serialization;

namespace Infrastructure.Entities.Delius;

public partial class AdditionalIdentifier
{
    public long? OffenderId { get; set; }

    public long Id { get; set; }

    public string? Pnc { get; set; }

    public string? Yot { get; set; }

    public string? OldPnc { get; set; }

    public string? MilitaryServiceNumber { get; set; }

    public string? Deleted { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual Offender Offender { get; set; } = null!;
    #endregion
}
