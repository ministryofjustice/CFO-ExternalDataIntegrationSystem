namespace API.DTOs.Delius
{
    /// <summary>
    /// Represent a view of an offenders offences.
    /// </summary>
    public record OffenceDto
    {
        /// <summary>
        /// The Case Reference Number (from delius)
        /// </summary>
        public required string Crn { get; set; } 

        /// <summary>
        /// A collection of main offences.
        /// </summary>
        public required MainOffenceDto[] MainOffences { get; set; } = [];

        /// <summary>
        /// Flag that indicates the record has been soft deleted in Delius
        /// </summary>
        public bool IsDeleted { get; set ; }
    }
}
