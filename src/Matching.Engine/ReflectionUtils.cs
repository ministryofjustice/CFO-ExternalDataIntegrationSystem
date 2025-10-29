using Matching.Core.Attributes;
using Matching.Engine.Scoring.Attributes;

namespace Matching.Engine;

public class ReflectionUtils
{
    public static Assembly[] GetMatchingAssemblies() => [Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(IMatcher))];

    public static Type[] GetMatchingTypes<T>() => GetMatchingAssemblies()
        .Select(x => x.GetTypes()
            .Where(t => t.IsAssignableTo<T>() && !t.IsAbstract && t.IsClass))
        .SelectMany(x => x)
        .ToArray();

    public static string GetMatchingTypeKey(Type t) => t.GetCustomAttribute<MatcherAttribute>()?.Key ?? throw new Exception($"Missing 'MatchingAttribute' on {t.FullName}.");
    public static string GetBayesTypeKey(Type t) => t.GetCustomAttribute<BayesAttribute>()?.Key ?? throw new Exception($"Missing 'BayesAttribute' on {t.FullName}.");
}
