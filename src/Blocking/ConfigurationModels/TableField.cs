namespace Blocking.ConfigurationModels;

public class TableField
{
    public string FieldName { get; set; } = string.Empty;
    public CleaningConfiguration? CleaningConfig { get; init; }
}
