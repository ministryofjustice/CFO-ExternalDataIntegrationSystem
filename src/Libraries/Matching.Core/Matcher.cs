namespace Matching.Core;

public abstract class Matcher<T, Result> : IMatcher
    where Result : MatcherResult
{
    protected abstract Result Match(T? source, T? target);

    public MatcherResult Match(object? sourceObject, object? targetObject)
    {
        // Are both the source and target null?
        if (sourceObject is null && targetObject is null)
        {
            return Match(default, default);
        }

        // Are the source and target of type T, or are either null?
        if ((sourceObject is T || sourceObject is null) && (targetObject is T || targetObject is null))
        {
            return Match((T)sourceObject, (T)targetObject);
        }

        if (typeof(T) == typeof(string))
        {
            sourceObject = sourceObject?.ToString();
            targetObject = targetObject?.ToString();
            return Match((T)sourceObject, (T)targetObject);
        }

        if (sourceObject is not T)
        {
            throw new ArgumentException($"Unable to resolve type from {sourceObject?.GetType()} to {typeof(T)}");
        }

        if (targetObject is not T)
        {
            throw new ArgumentException($"Unable to resolve type from {targetObject?.GetType()} to {typeof(T)}");
        }

        throw new ArgumentException($"Unable to resolve any suitable type for {typeof(T)}");
    }

}
