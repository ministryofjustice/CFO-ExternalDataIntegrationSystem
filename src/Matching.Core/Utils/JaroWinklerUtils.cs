using SimMetrics.Net.Metric;

namespace Matching.Core.Utils;

public static class JaroWinklerUtils
{
    public static double GetJaroWinklerSimilarity(string? source, string? target)
    {
        if (source is null || target is null)
        {
            return 0;
        }

        if (StringUtils.Equal(source, target))
        {
            return 1;
        }

        JaroWinkler jaroWinkler = new JaroWinkler();
        return jaroWinkler.GetSimilarity(source, target);
    }

}