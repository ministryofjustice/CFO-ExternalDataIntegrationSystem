using Matching.Core.Attributes;
using Matching.Core.Matchers.Results;
using System.Text.RegularExpressions;

namespace Matching.Core.Matchers;

[Matcher("Postcode")]
public class PostcodeMatcher : Matcher<string, PostcodeMatcherResult>
{
    protected override PostcodeMatcherResult Match(string? source, string? target)
    {
        // Slow & messy.
        // Needs work.

        if (source is null || target is null)
            return new PostcodeMatcherResult() { Source = source, Target = target };

        if (source == target)
            return PostcodeMatcherResult.Identical(source);

        var levenstein = new LevenshteinMatcher()
            .Match(source, target) as LevenshteinMatcherResult;

        if (levenstein.LevenshteinEditDistance is 0)
            return PostcodeMatcherResult.Identical(source);

        var regex = new Regex("");

        var sourceMatches = regex.Match(source);

        if (sourceMatches.Success is false)
            throw new ArgumentException("Invalid format for postcode.", nameof(source));

        var targetMatches = regex.Match(source);

        if (targetMatches.Success is false)
            throw new ArgumentException("Invalid format for postcode.", nameof(target));

        var sourceElements = GetElements(source, sourceMatches.Groups);
        var targetElements = GetElements(target, sourceMatches.Groups);

        var result = new PostcodeMatcherResult
        {
            Source = source,
            Target = target,
            LevenshteinEditDistance = levenstein.LevenshteinEditDistance,
            SameOutwardCode = sourceElements[0].Equals(targetElements[0]),
            SameInwardCode = sourceElements[1].Equals(targetElements[1]),
            SameAreaCode = sourceElements[2].Equals(targetElements[2]),
            SameDistrictCode = sourceElements[3].Equals(targetElements[3]),
            SameSubdistrictCode = sourceElements[4].Equals(targetElements[4]),
            SameSector = sourceElements[5].Equals(targetElements[5]),
            SameUnit = sourceElements[6].Equals(targetElements[6])
        };

        return result;
    }

    private static string[] GetElements(string value, GroupCollection groups)
    {
        string outward = value.Split(" ")[0];
        string inward = value.Split(" ")[1];
        string area = groups[1].Value;
        string district = outward;
        string subdistrict = string.Empty;
        string sector = $"{district} {inward[0]}";
        string unit = groups[4].Value;

        if (char.IsLetter(outward[^1]))
        {
            district = outward.Substring(0, outward.Length - 1);
            subdistrict = outward;
        }

        return [outward, inward, area, district, subdistrict, sector, unit];
    }

}
