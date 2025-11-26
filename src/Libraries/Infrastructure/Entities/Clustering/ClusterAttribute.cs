namespace Infrastructure.Entities.Clustering;

public class ClusterAttribute
{
    public int ClusterId { get; set; }
    public string UPCI { get; set; }
    public string RecordSource { get; set; }
    public string Identifier { get; set; }
    public bool PrimaryRecord { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
}
