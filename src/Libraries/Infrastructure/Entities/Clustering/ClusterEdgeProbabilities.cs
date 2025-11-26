namespace Infrastructure.Entities.Clustering;

public class ClusterEdgeProbabilities
{
    public int TempClusterId { get; set; }
    public string SourceName { get; set; }
    public string SourceKey { get; set; }
    public string TargetName { get; set; }
    public string TargetKey { get; set; }
    public double Probability { get; set; }
}
