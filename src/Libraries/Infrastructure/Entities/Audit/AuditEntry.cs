namespace Infrastructure.Entities.Audit;

public class AuditEntry
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private AuditEntry() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private AuditEntry(Guid correlationId, string entityName, string action, string? performedBy, string keyValues, string? oldValues, string? newValues, DateTime? timestamp)
    {
        Id = Guid.CreateVersion7();
        CorrelationId = correlationId;
        EntityName = entityName;
        Action = action;
        Timestamp = timestamp ?? DateTime.UtcNow;
        PerformedBy = performedBy;
        KeyValues = keyValues;
        OldValues = oldValues;
        NewValues = newValues;
    }

    public Guid Id { get; private set; } 
    public Guid CorrelationId { get; private set; }
    public string EntityName { get; private set; }
    public string Action { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? PerformedBy { get; private set; }
    public string KeyValues { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }

    public static AuditEntry Create(Guid correlationId, string entityName, string action, string? performedBy, string keyValues, string? oldValues, string? newValues, DateTime? timestamp)
        => new(correlationId, entityName, action, performedBy, keyValues, oldValues, newValues, timestamp);

}
