using Infrastructure.Contexts;
using Infrastructure.DTOs;
using Infrastructure.Entities.Clustering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Metrics;

namespace Infrastructure.Repositories.Clustering;

public class ClusteringRepository(
    ClusteringContext context) : IClusteringRepository
{
    public async Task<Cluster?> GetByIdAsync(int id)
    {
        return await context.Clusters
            .Include(e => e.Members)
            .SingleOrDefaultAsync(e => id == e.ClusterId);
    }

    public async Task<Cluster?> GetByUpciAsync(string upci) =>
        await context.Clusters
            .Include(e => e.Members)
            .SingleOrDefaultAsync(e => upci == e.UPCI);

    public async Task<ClusterAttribute[]> SearchAsync(string identifier, string lastName, DateOnly dateOfBirth) =>
        await context.Clusters
            .SelectMany(e => e.Attributes)
            .Where(e => e.Identifier == identifier)
            .Union
            (
                context.Clusters.SelectMany(e => e.Attributes)
                    .Where(e => e.LastName == lastName && e.DateOfBirth == dateOfBirth)
            )
            .AsNoTracking()
            .ToArrayAsync();

    public async Task<string?> GetStickyLocation(string upci)
    {
        var sticky = await context.StickyLocations
            .Where(e => e.Upci == upci)
            .SingleOrDefaultAsync();

        return sticky switch
        {
            null => null,
            _ => sticky.OrgCode
        };


    }

    public async Task<Cluster?> GenerateClusterAsync()
    {
        // Most recently inserted cluster
        var id = await context.Clusters
            .Select(x => x.ClusterId)
            .DefaultIfEmpty()
            .MaxAsync();

        // Id of new cluster
        var newId = id + 1;

        var reference = await context.UPCI2s
            .Where(u => u.ClusterId == newId)
            .Select(u => new 
            {
                Id = u.ClusterId,
                u.Upci
            }).FirstOrDefaultAsync();

        // There are no empty clusters
        if (reference is null)
        {
            return null;
        }

        // Populate new cluster
        var cluster = new Cluster()
        {
            RecordCount = 0,
            ContainsInternalDupe = false,
            ContainsLowProbabilityMembers = false,
            ClusterId = reference.Id,
            UPCI = reference.Upci,
        };

        context.Clusters.Add(cluster);

        await context.SaveChangesAsync();

        return cluster;
    }
}
