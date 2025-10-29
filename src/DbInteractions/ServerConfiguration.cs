
namespace DbInteractions;

public class ServerConfiguration
{
    public string DeliusStagingProcedure { get; init; } = string.Empty;
    public string OfflocStagingProcedure { get; init; } = string.Empty;
    public string DeliusRunningPictureMergeProcedure { get; init; } = string.Empty;
    public string OfflocRunningPictureMergeProcedure { get; init; } = string.Empty;
    public string DeliusClearStagingProcedure {  get; init; } = string.Empty;
    public string OfflocClearStagingProcedure {  get; init; } = string.Empty;
    public string DeliusStagingStandardiseDataProcedure { get; init; } = string.Empty;
}
