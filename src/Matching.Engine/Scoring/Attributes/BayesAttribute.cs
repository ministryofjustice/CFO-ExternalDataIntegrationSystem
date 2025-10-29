namespace Matching.Engine.Scoring.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BayesAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}

