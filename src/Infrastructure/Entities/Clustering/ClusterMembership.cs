namespace Infrastructure.Entities.Clustering;

public class ClusterMembership
{
    public int ClusterId { get; set; }
    public string NodeName { get; set; }
    public string NodeKey { get; set; }
    public decimal ClusterMembershipProbability { get; set; }
    public bool HardLink { get; set; }

    public virtual ICollection<ClusterEdgeProbabilities> EdgeProbabilities { get; set; } = [];
}
