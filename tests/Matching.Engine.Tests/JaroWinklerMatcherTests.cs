using Matching.Core.Matchers;
using Matching.Core.Matchers.Results;

namespace Matching.Engine.Tests;

public class JaroWinklerMatcherTests
{
    private readonly JaroWinklerMatcher _matcher;

    public JaroWinklerMatcherTests()
    {
        _matcher = new JaroWinklerMatcher();
    }

    [Fact]
    public void Match_IdenticalStrings_ReturnsOne()
    {
        // Arrange
        var source = "hello";
        var target = "hello";

        // Act
        var result = _matcher.Match(source, target) as JaroWinklerMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1.0, result.JaroWinklerSimilarity);
        Assert.Equal(source, result.Source);
        Assert.Equal(target, result.Target);
    }

    [Fact]
    public void Match_CompletelyDifferentStrings_ReturnsLowSimilarity()
    {
        // Arrange
        var source = "abc";
        var target = "xyz";

        // Act
        var result = _matcher.Match(source, target) as JaroWinklerMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.JaroWinklerSimilarity < 0.5);
    }

    [Fact]
    public void Match_SimilarStrings_ReturnsHighSimilarity()
    {
        // Arrange
        var source = "martha";
        var target = "marhta";

        // Act
        var result = _matcher.Match(source, target) as JaroWinklerMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.JaroWinklerSimilarity > 0.9);
    }

    [Theory]
    [InlineData("JONES", "JOHNSON")]
    [InlineData("SMITH", "SMYTH")]
    [InlineData("DIXON", "DICKSON")]
    public void Match_PhoneticallySimilar_ReturnsReasonableSimilarity(
        string source, string target)
    {
        // Act
        var result = _matcher.Match(source, target) as JaroWinklerMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.JaroWinklerSimilarity > 0.5, 
            $"Expected similarity > 0.5 for {source} and {target}, got {result.JaroWinklerSimilarity}");
    }

    [Fact]
    public void Match_NullStrings_ReturnsZeroSimilarity()
    {
        // Act
        var result = _matcher.Match(null, null) as JaroWinklerMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0.0, result.JaroWinklerSimilarity);
        Assert.True(result.MissingInSource);
        Assert.True(result.MissingInTarget);
    }

    [Fact]
    public void Match_CaseInsensitive_TreatsAsEqual()
    {
        // Arrange
        var source = "HELLO";
        var target = "hello";

        // Act
        var result = _matcher.Match(source, target) as JaroWinklerMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1.0, result.JaroWinklerSimilarity);
    }
}
