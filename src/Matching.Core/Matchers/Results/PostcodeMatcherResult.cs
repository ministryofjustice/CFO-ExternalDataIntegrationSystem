namespace Matching.Core.Matchers.Results;

public class PostcodeMatcherResult : MatcherResult, ILevenshteinStringMatcherResult
{
    public int LevenshteinEditDistance { get; set; }

    /// <summary>
    /// Whether the last two letters of the postcode are the same.
    /// </summary>
    public bool SameUnit { get; set; }

    /// <summary>
    /// Whether the postcode district and first character of the inward code are the same.
    /// </summary>
    public bool SameSector { get; set; }

    /// <summary>
    /// Whether the first half of a postcode is the same.
    /// </summary>
    public bool SameOutwardCode { get; set; }

    /// <summary>
    /// Whether the second half of a postcode is the same.
    /// </summary>
    public bool SameInwardCode { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool SameAreaCode { get; set; }

    /// <summary>
    /// Whether part of the outward code is the same.
    /// </summary>
    public bool SameDistrictCode { get; set; }

    /// <summary>
    /// Whether part of the outward code including the trailing letter omitted 
    /// from the district code is the same. Very uncommon.
    /// </summary>
    public bool SameSubdistrictCode { get; set; }

    public static PostcodeMatcherResult Identical(string source) => new()
    {
        Source = source,
        Target = source,
        SameAreaCode = true,
        SameInwardCode = true,
        SameOutwardCode = true,
        SameSector = true,
        SameUnit = true,
        SameDistrictCode = true,
        SameSubdistrictCode = true,
        LevenshteinEditDistance = 0
    };
}