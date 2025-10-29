using System.Text.Json.Serialization;

namespace Infrastructure.Entities.Aggregation;

public class ClusterAggregate
{
    public required string Identifier { get; set; }
    public required string FirstName { get; set; }
    public string? SecondName { get; set; }
    public required string LastName { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public string? NomisNumber { get; set; }
    public string? PrisonNumber { get; set; }
    public string? PncNumber { get; set; }
    public string? Crn { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public string? Ethnicity { get; set; }
    public string Primary { get; set; }
    public bool IsActive { get; set; }
    public string? OrgCode { get; set; }
    public string? EstCode { get; set; }

    public string? StickyLocation { get; set; }
    public IEnumerable<string?> RegistrationDetails { get; set; } = [];

    [JsonIgnore]
    public DateTime ValidFrom { get; set; }

    [JsonIgnore]
    public DateTime ValidTo { get; set; }

    public string? PrimaryRecordKey => Primary is "NOMIS" ? NomisNumber : Crn;
}
