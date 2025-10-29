using System.Collections.Concurrent;

namespace Matching.Engine.Cache;

public class MatcherCache(IComponentContext context)
{
    private readonly ConcurrentDictionary<string, IMatcher> cache = [];
    public IMatcher Resolve(string key) => cache.GetOrAdd(key, context.ResolveKeyed<IMatcher>);
}
