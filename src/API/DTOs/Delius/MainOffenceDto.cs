namespace API.DTOs.Delius;

/// <summary>
/// Represents a 'Main' offence in delius
/// </summary>
public record MainOffenceDto
{
    /// <summary>
    /// The description of the offence
    /// </summary>
    public required string OffenceDescription { get; set; }

    /// <summary>
    /// The date of the offence
    /// </summary>
    public DateOnly? OffenceDate { get; set; }

    /// <summary>
    /// A collection of 0 or more disposals linked to an offence.
    /// </summary>
    public required DisposalDto[] Disposals { get; set; } = [];

    /// <summary>
    /// A flag to indicate if the offence has been soft deleted
    /// in delius
    /// </summary>
    public bool IsDeleted { get; set; }
}