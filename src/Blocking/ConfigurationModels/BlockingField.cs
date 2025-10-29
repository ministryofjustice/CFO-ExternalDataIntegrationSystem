using Blocking.ConfigurationModels.Enums;

namespace Blocking.ConfigurationModels;

public class BlockingField
{
    public TFieldType FieldType { get; init; }
    public TableField OfflocField { get; init; } = new();
    public TableField DeliusField { get; init; } = new();
}
