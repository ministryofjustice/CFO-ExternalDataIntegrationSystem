namespace Matching.Engine.Scoring;

public static class ScoreUtils
{
    public static bool IsWhole(this MatcherResult matcherResult) => !matcherResult.MissingInSource || !matcherResult.MissingInTarget;
}
