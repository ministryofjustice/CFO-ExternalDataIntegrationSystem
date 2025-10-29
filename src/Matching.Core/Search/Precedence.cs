using Matching.Core.Matchers.Results;
using Matching.Core.Matchers;

namespace Matching.Core.Search;

public static class Precedence
{
    static readonly IReadOnlyDictionary<string, int> precedenceMap = new Dictionary<string, int>
    {
        { "111", 1 },   // Identical, Identical, Identical
        { "121", 2 },   // Identical, Similar, Identical
        { "112", 3 },   // Identical, Identical, Similar
        { "211", 4 },   // Similar, Identical, Identical
        { "101", 5 },   // Identical, Different, Identical
        { "122", 6 },   // Identical, Similar, Similar
        { "110", 7 },   // Identical, Identical, Different
        { "102", 8 },   // Identical, Different, Similar
        { "120", 9 },   // Identical, Similar, Different
        { "011", 10 },  // Different, Identical, Identical
        { "100", 11 },  // Identical, Different, Different
    };

    static readonly dynamic matchers = new
    {
        Levenshtein = new LevenshteinMatcher(),
        Caver = new CaverMatcher(),
        JaroWinkler = new JaroWinklerMatcher(),
        Date = new DateMatcher()
    };

    private static int GetKey(bool[] comparison)
    {
        string key = $"{(comparison[0] ? 1 : comparison[1] ? 2 : 0)}" +
                     $"{(comparison[2] ? 1 : comparison[3] ? 2 : 0)}" +
                     $"{(comparison[4] ? 1 : comparison[5] ? 2 : 0)}";

        return precedenceMap.ContainsKey(key) ? precedenceMap[key] : 99;
    }

    public static int GetPrecedence((string left, string right) identifiers, (string left, string right) lastNames, (DateOnly left, DateOnly right) dateOfBirths)
    {
        bool[] comparisons = new bool[6];

        // 0: Identifier (same)
        // 1: Identifier (similar)
        // 2: Last Name (same)
        // 3: Last Name (similar)
        // 4: Date of Birth (same)
        // 5: Date of Birth (similar)

        comparisons[0] = comparisons[1] = string.Equals(identifiers.left, identifiers.right, StringComparison.OrdinalIgnoreCase);
        comparisons[2] = comparisons[3] = string.Equals(lastNames.left, lastNames.right, StringComparison.OrdinalIgnoreCase);
        comparisons[4] = comparisons[5] = (dateOfBirths.left == dateOfBirths.right);

        if (comparisons.All(c => c))
        {
            return GetKey(comparisons);
        }

        // Are the identifiers different?
        if (!comparisons[0])
        {
            var levenshteinMatcherResult = matchers.Levenshtein
                .Match(identifiers.left, identifiers.right) as LevenshteinMatcherResult;

            comparisons[1] = levenshteinMatcherResult.LevenshteinEditDistance <= 2;
        }

        // Are the last names different?
        if (!comparisons[2])
        {
            var jaroWinklerMatcherResult = matchers.JaroWinkler
                .Match(lastNames.left, lastNames.right) as JaroWinklerMatcherResult;

            var caverMatcherResult = matchers.Caver
                .Match(lastNames.left, lastNames.right) as CaverMatcherResult;

            comparisons[3] = jaroWinklerMatcherResult.JaroWinklerSimilarity >= 0.85 || caverMatcherResult.IsPhoneticallySimilar;
        }

        // Are the date of births different?
        if (!comparisons[4])
        {
            var dateMatcherResult = matchers.Date
                .Match(dateOfBirths.left.ToDateTime(TimeOnly.MinValue), dateOfBirths.right.ToDateTime(TimeOnly.MinValue)) as DateMatcherResult;

            comparisons[5] =
                (dateMatcherResult.SameYear && dateMatcherResult.SameMonth) ||
                (dateMatcherResult.SameMonth && dateMatcherResult.SameDay) ||
                (dateMatcherResult.SameYear && dateMatcherResult.SameDay) ||
                (dateMatcherResult.SameYear && dateMatcherResult.DayAndMonthTransposed);
        }

        return GetKey(comparisons);
    }
}
