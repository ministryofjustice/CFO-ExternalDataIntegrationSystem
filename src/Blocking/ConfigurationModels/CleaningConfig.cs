using Blocking.ConfigurationModels.Enums;

namespace Blocking.ConfigurationModels;

public class CleaningConfiguration
{
    public TCleaningType CleaningType { get; init; }
    public int? TrimAmount { get; init; }
}
