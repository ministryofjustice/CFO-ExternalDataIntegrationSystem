namespace Matching.Core.Utils;

internal static class LevenshteinUtils
{
    /// <summary>
    /// Calculates the levenshtein distance between the <paramref name="source"/> and <paramref name="target"/>.
    /// </summary>
    /// <param name="source">the source string.</param>
    /// <param name="target">the target string.</param>
    /// <returns></returns>
    internal static int CalculateDistance(string? source, string? target)
    {
        source ??= string.Empty;
        target ??= string.Empty;

        if (StringUtils.Equal(source, target))
        {
            return 0;
        }

        int n = source.Length;
        int m = target.Length;

        if (n == 0)
        {
            return m;
        }

        if (m == 0)
        {
            return n;
        }

        int[,] d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; d[i, 0] = i++) ;
        for (int j = 0; j <= m; d[0, j] = j++) ;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }
}