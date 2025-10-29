namespace API.DTOs.Delius;

public record OffenderManagerSummaryDto
{
    public string? OrganisationCode { get; set; } = default;
    public string? OrganistationDescription { get; set; } = default;
    public string? TeamCode { get; set; } = default;
    public string? TeamDescription { get; set; } = default;

}
