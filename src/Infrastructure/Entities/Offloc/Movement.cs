namespace Infrastructure.Entities.Offloc;

public partial class Movement
{
    public string NomsNumber { get; set; } = null!;

    public string? MovementEstabComponent { get; set; }

    public string? MovementCode { get; set; }

    public string? TransferReason { get; set; }

    public DateOnly? DateOfMovement { get; set; }

    public int? HourOfMovement { get; set; }

    public int? MinOfMovement { get; set; }

    public int? SecOfMovement { get; set; }

    public bool IsActive { get; set; }

    #region Relationships
    [JsonIgnore]
    public virtual PersonalDetail PersonalDetail { get; set; } = null!;
    #endregion
}
