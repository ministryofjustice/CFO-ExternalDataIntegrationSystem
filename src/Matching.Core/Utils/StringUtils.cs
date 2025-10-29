namespace Matching.Core.Utils;
public static class StringUtils
{
    public static bool Equal(string? source, string? target)
    {
        return string.Equals(source, target, StringComparison.OrdinalIgnoreCase)
               && source?.Length > 0
               && target?.Length > 0;
    }
}
