using Infrastructure.Contexts;
using Infrastructure.DTOs;
using Infrastructure.Entities.Clustering;
using System.Runtime.InteropServices;

namespace Infrastructure.Repositories.Visualisation;

public class VisualisationRepository(
    ClusteringContext context,
    DeliusContext deliusContext,
    OfflocContext offlocContext) : IVisualisationRepository
{

    public async Task<ClusterDto?> GetDetailsByUpciAsync(string upci)
    {
        var query =
            from c in context.Clusters
            where c.UPCI == upci
            select new
            {
                c.UPCI,
                c.ClusterId,
                Members = c.Members.Select(m => new
                {
                    m.NodeKey,
                    m.NodeName,
                    m.HardLink,
                    Edges = m.EdgeProbabilities.Select(e => new
                    {
                        e.SourceKey,
                        e.TargetKey,
                        e.Probability
                    })

                }).ToList()
            };

        var cluster = await query.SingleOrDefaultAsync();

        // Not found
        if (cluster is null)
        {
            return null;
        }

        // Cluster has no members -> return the cluster itself as a single node
        if (cluster is { Members.Count: 0 })
        {
            return ClusterDto.Empty(cluster.UPCI);
        }

        var deliusNodes = await GetDeliusNodeMetadataAsync(
            cluster.Members.Where(m => m.NodeName == "DELIUS").Select(m => m.NodeKey));

        var offlocNodes = await GetOfflocNodeMetadataAsync(
            cluster.Members.Where(m => m.NodeName == "NOMIS").Select(m => m.NodeKey));

        var metadata = deliusNodes.Union(offlocNodes).ToDictionary(k => k.Key);

        return new ClusterDto
        {
            UPCI = cluster.UPCI,
            Nodes = cluster.Members.Select(m => new NodeDto
            {
                Id = m.NodeKey,
                Group = cluster.UPCI,
                Source = m.NodeName,
                HardLink = m.HardLink,
                Metadata = metadata.GetValueOrDefault(m.NodeKey)
            }),
            Edges = cluster.Members.SelectMany(m => m.Edges).Select(e => new EdgeDto
            {
                From = e.SourceKey,
                To = e.TargetKey,
                Probability = e.Probability
            })
        };

    }

    public async Task<bool> SaveNetworkAsync(NetworkDto network)
    {

        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                string[] upcis = network.Clusters.Select(c => c.UPCI).ToArray();

                var existingClusters = await context.Clusters
                    .Where(c => upcis.Contains(c.UPCI))
                    .ToListAsync();

                if (existingClusters.Count != network.Clusters.Count())
                {
                    throw new Exception("Cluster count mismatch");
                }

                if (existingClusters.SelectMany(e => e.Members).Count() != network.Clusters.SelectMany(e => e.Nodes).Count())
                {
                    throw new Exception("Cluster members count mismatch");
                }

                // Verify if any edges have been added to nodes not in the same cluster

                // Verify if any edges have been drawn to nodes that do not exist

                var nodeMapping = existingClusters.SelectMany(e => e.Members)
                    .ToDictionary(e => e.NodeKey, v => v.NodeName);

                var attributeMapping = existingClusters.SelectMany(e => e.Attributes)
                    .ToDictionary(e => e.Identifier, v => new
                    {
                        v.LastName,
                        v.DateOfBirth
                    });

                var updatedClusters = existingClusters.Select(source =>
                {
                    var cluster = network.Clusters
                        .First(c => c.UPCI == source.UPCI);

                    var target = new Cluster
                    {
                        ClusterId = source.ClusterId,
                        UPCI = source.UPCI,
                        IdentifiedOn = source.IdentifiedOn,
                        Members = cluster.Nodes.Select(n => new ClusterMembership
                        {
                            ClusterId = source.ClusterId,
                            HardLink = n.HardLink,
                            NodeKey = n.Id,
                            ClusterMembershipProbability = -1,
                            NodeName = n.Source!,
                            EdgeProbabilities = cluster.Edges.Where(e => e.From == n.Id).Select(e => new ClusterEdgeProbabilities
                            {
                                Probability = e.Probability,
                                SourceKey = e.From,
                                SourceName = nodeMapping[e.From],
                                TargetKey = e.To,
                                TargetName = nodeMapping[e.To],
                                TempClusterId = -1 // Completely meaningless
                            }).ToList()
                        }).ToList(),
                        Attributes = cluster.Nodes.Select(n => new ClusterAttribute
                        {
                            ClusterId = source.ClusterId,
                            UPCI = source.UPCI,
                            DateOfBirth = attributeMapping[n.Id].DateOfBirth,
                            LastName = attributeMapping[n.Id].LastName,
                            RecordSource = nodeMapping[n.Id],
                            Identifier = n.Id,
                            PrimaryRecord = false
                        }).ToList()
                    };

                    return target;
                }).ToList();

                context.RemoveRange(existingClusters);

                // Update metadata
                foreach(var cluster in updatedClusters)
                {
                    var primary = await GetPrimaryRecordAsync(cluster);
                    cluster.UpdateMetadata(primary);
                }

                context.AddRange(updatedClusters);

                await context.SaveChangesAsync();

                await transaction.CommitAsync();
            }

            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        return await Task.FromResult(true);
    }

    private async Task<IEnumerable<NodeMetadataDto>> GetDeliusNodeMetadataAsync(params IEnumerable<string> crns)
    {
        var query =
            from o in deliusContext.Offenders
            where crns.Contains(o.Crn)
            select new NodeMetadataDto
            {
                Key = o.Crn,
                IsActive = o.IsActive,
                FirstName = o.FirstName,
                MiddleName = o.SecondName,
                LastName = o.Surname,
                DateOfBirth = o.DateOfBirth,
                Gender = o.GenderDescription,
                PncNumbers =
                    (o.Pncnumber != null ? new[] { o.Pncnumber } : Array.Empty<string>())
                    .Concat(
                        deliusContext.AdditionalIdentifiers
                            .Where(e => e.OffenderId == o.OffenderId && e.Pnc != null)
                            .Select(e => e.Pnc!)
                    )
                    .ToArray(),
                CroNumbers = o.Cro != null ? new string[] { o.Cro } : Array.Empty<string>(),
                NomisNumbers = o.Nomisnumber != null ? new string[] { o.Nomisnumber } : Array.Empty<string>(),
            };

        return await query.ToListAsync();
    }

    private async Task<IEnumerable<NodeMetadataDto>> GetOfflocNodeMetadataAsync(params IEnumerable<string> nomisNumbers)
    {
        var query =
            from pd in offlocContext.PersonalDetails
            where nomisNumbers.Contains(pd.NomsNumber)
            select new NodeMetadataDto
            {
                Key = pd.NomsNumber,
                IsActive = pd.IsActive,
                FirstName = pd.FirstName,
                MiddleName = pd.SecondName,
                LastName = pd.Surname,
                DateOfBirth = pd.DateOfBirth,
                Gender = pd.Gender,
                CroNumbers = offlocContext.Identifiers
                    .Where(i => i.NomsNumber == pd.NomsNumber)
                    .Select(i => i.Crono)
                    .ToArray(),
                PncNumbers = offlocContext.Pncs
                    .Where(p => p.NomsNumber == pd.NomsNumber)
                    .Select(i => i.Details)
                    .ToArray(),
                NomisNumbers = new string[] { pd.NomsNumber }
            };

        return await query.ToListAsync();
    }

    private async Task<ClusterMembership?> GetPrimaryRecordAsync(Cluster cluster)
    {
        if(cluster.Members is not { Count: > 0 })
        {
            return null;
        }

        var crns = cluster.Members.Where(o => o.NodeName == "DELIUS").Select(o => o.NodeKey).ToArray();
        var nomisNumbers = cluster.Members.Where(o => o.NodeName == "NOMIS").Select(o => o.NodeKey).ToArray();

        var deliusRecords = await deliusContext.Offenders
            .Where(o => crns.Contains(o.Crn))
            .Select(o => new
            {
                Id = o.Crn,
                o.IsActive,
                Source = "DELIUS"
            }).ToListAsync();

        var nomisRecords = await offlocContext.PersonalDetails
            .Where(o => nomisNumbers.Contains(o.NomsNumber))
            .Select(pd => new
            {
                Id = pd.NomsNumber,
                pd.IsActive,
                Source = "NOMIS"
            }).ToListAsync();

        var records = deliusRecords.Concat(nomisRecords);

        var primary = records
            .OrderByDescending(r => r.IsActive)
            .ThenByDescending(r => r.Source == "NOMIS")
            .ThenByDescending(r => r.Id)
            .First();

        return cluster.Members.First(m => m.NodeKey == primary.Id);
    }
}
