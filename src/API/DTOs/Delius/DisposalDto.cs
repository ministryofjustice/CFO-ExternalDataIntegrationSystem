namespace API.DTOs.Delius;

/// <summary>
/// Represents a disposal
/// </summary>
public record DisposalDto
{
    /// <summary>
    /// The sentence date
    /// </summary>
    public DateOnly? SentenceDate { get; set; }

    /// <summary>
    /// The length of time the disposal is valid for
    /// </summary>
    public required string Length { get; set; }

    /// <summary>
    /// A description of the unit (for example M for months or H for hours)
    /// </summary>
    public required string UnitDescription { get; set; }

    /// <summary>
    /// The primary details of the disposal.
    /// </summary>
    public required string DisposalDetail { get; set; }

    /// <summary>
    /// A collection of 0 or more requirements.
    /// </summary>
    public required RequirementDto[] Requirements { get; set; } = [];

    /// <summary>
    /// The reason for termination (if terminated)
    /// </summary>
    public string? TerminationDescription { get; set; }

    /// <summary>
    /// The termination date (if terminated)
    /// </summary>
    public DateOnly? TerminationDate { get; set; }
    
    /// <summary>
    /// A flag to indicate if the disposal has been soft deleted in
    /// delius
    /// </summary>
    public bool IsDeleted { get; set; }
}