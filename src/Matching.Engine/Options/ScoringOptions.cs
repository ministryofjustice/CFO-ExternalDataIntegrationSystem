namespace Matching.Engine.Options;

public class MatchingOption
{
    public string MatchingKey { get; set; } = string.Empty;
    public IEnumerable<ComparisonField> ComparatorOptions { get; set;}
    public ScoringOptions ScoringOptions { get; set; }
}

public class ScoringOptions
{
    public double Prior { get; set; } = 0.00;
    public IEnumerable<FieldOption> Fields { get; set; } = [];
}

public class FieldOption
{
    public string FieldKey { get; set; } = string.Empty;
    public List<BayesFactorOption> BayesFactors { get; set; } = [];
}

public class BayesFactorOption
{
    public string ComparatorKey { get; set; } = string.Empty;
    public Dictionary<string, double> Bayes { get; set; } = [];

}