namespace Matching.Core;

public abstract class MatcherResult
{
    public required object? Source { get; set; }
    public required object? Target { get; set; }
    public virtual bool MissingInSource => Source is null;
    public virtual bool MissingInTarget => Target is null;
    public bool MissingInBoth => MissingInSource && MissingInTarget;
}
