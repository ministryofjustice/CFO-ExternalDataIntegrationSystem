using Infrastructure.Entities.Aggregation;

namespace API.Extensions;

public static class AggregateExtensions
{
    public static IEnumerable<ClusterAggregate> OrderByHierarchy<T>(this IEnumerable<ClusterAggregate> source, Func<ClusterAggregate, T> keySelector) => source
        .OrderByDescending(keySelector)
        .ThenByDescending(e => e.IsActive)
        .ThenByDescending(e => e.Primary is "NOMIS")
        .ThenByDescending(e => e.ValidFrom)
        .ThenByDescending(e => e.Primary is "NOMIS" ? e.NomisNumber : e.Crn);
}
