using Matching.Engine.Scoring;
using System.Collections.Concurrent;

namespace Matching.Engine.Cache;

public class BayesCache(IComponentContext context)
{
    private readonly ConcurrentDictionary<string, IBayesFactor> bayesCache = [];

    public IBayesFactor Resolve(string key) => bayesCache.GetOrAdd(key, context.ResolveKeyed<IBayesFactor>);
}
