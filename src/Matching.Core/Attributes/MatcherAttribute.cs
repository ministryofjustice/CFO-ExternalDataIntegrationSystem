namespace Matching.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MatcherAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}
