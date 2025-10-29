namespace Matching.Core;

public interface IMatcher<T, Result>
    where Result : MatcherResult
{
    Result Match(T source, T target);
}

public interface IMatcher
{
    MatcherResult Match(object? source, object? target);
}

/*
public interface IMatcherWithOptions<T, Result, Options> : IMatcher<T, Result>
    where Result : MatcherResult
{
    Result Match(T target, Options options);
}
*/