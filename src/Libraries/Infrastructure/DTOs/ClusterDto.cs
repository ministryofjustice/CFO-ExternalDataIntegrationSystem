namespace Infrastructure.DTOs;

public class ClusterDto
{
    public required string UPCI { get; set; }

    public IEnumerable<NodeDto> Nodes { get; set; } = [];
    public IEnumerable<EdgeDto> Edges { get; set; } = [];

    public static ClusterDto Empty(string upci)
        => new()
        {
            UPCI = upci,
            Nodes =
            [
                new NodeDto
                    {
                        Id =  upci,
                        Group = upci,
                        Type = "cluster"
                    }
            ]
        };
}
