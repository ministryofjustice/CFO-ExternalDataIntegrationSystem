namespace API.DTOs.Delius;

/// <summary>
/// Represents a requirement given by a court for a disposal.
/// </summary>
public record RequirementDto
{
    /// <summary>
    /// The category description
    /// </summary>
    public string CategoryDescription { get; set; }

    /// <summary>
    /// The sub category description
    /// </summary>
    public string SubCategoryDescription { get; set; }

    /// <summary>
    /// The termination reason (if terminated)
    /// </summary>
    public string TerminationDescription { get; set; }

    /// <summary>
    /// The length of the requirement.
    /// </summary>
    public string Length { get; set; }

    /// <summary>
    /// The unit of the length of requirement (for example H for hours)
    /// </summary>
    public string UnitDescription { get; set; }

    /// <summary>
    /// A flag to indicate if the requirement has been soft deleted
    /// in delius
    /// </summary>
    public bool IsDeleted { get; set; }

}