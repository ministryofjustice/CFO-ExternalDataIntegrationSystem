using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Infrastructure.Entities.Clustering;

public class Cluster
{
    public int ClusterId { get; set; }
    public string UPCI { get; set; }
    public short RecordCount { get; set; }
    public bool ContainsInternalDupe { get; set; }
    public bool ContainsLowProbabilityMembers { get; set; }
    public string? PrimaryRecordName { get; set; }
    public string? PrimaryRecordKey { get; set; }
    public DateTime? IdentifiedOn { get; set; }

    #region Relationships
    public virtual ICollection<ClusterMembership> Members { get; set; } = [];
    public virtual ICollection<ClusterAttribute> Attributes { get; set; } = [];
    #endregion

    public void UpdateMetadata(ClusterMembership? primaryRecord = null)
    {
        // Update cluster metadata
        RecordCount = (short)Members.Count;
        ContainsInternalDupe = Members.CountBy(m => m.NodeName).Any(c => c.Value > 1);
        ContainsLowProbabilityMembers = Members.SelectMany(m => m.EdgeProbabilities).Any(e => e.Probability < 0.9);

        // Update membership probabilities
        var membershipProbability = Members.Count > 1 ? Math.Sqrt(Members.SelectMany(m => m.EdgeProbabilities).Average(e => e.Probability)) : 1;
        foreach (var member in Members)
        {
            member.ClusterMembershipProbability = (decimal)membershipProbability;
        }

        // Update primary record
        if(primaryRecord is not null)
        {
            PrimaryRecordName = primaryRecord.NodeName;
            PrimaryRecordKey = primaryRecord.NodeKey;

            foreach(var attribute in Attributes)
            {
                attribute.PrimaryRecord = attribute.Identifier == PrimaryRecordKey;
            }

        }

    }

}
