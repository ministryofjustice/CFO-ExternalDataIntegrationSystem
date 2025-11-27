using Matching.Core.Matchers;
using Matching.Core.Matchers.Results;

namespace Matching.Engine.Tests;

public class CaverMatcherTests
{
    private readonly CaverMatcher _matcher;

    public CaverMatcherTests()
    {
        _matcher = new CaverMatcher();
    }

    [Fact]
    public void Match_IdenticalStrings_ReturnsTrue()
    {
        // Arrange
        var source = "smith";
        var target = "smith";

        // Act
        var result = _matcher.Match(source, target) as CaverMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsPhoneticallySimilar);
        Assert.Equal(source, result.Source);
        Assert.Equal(target, result.Target);
    }

    [Theory]
    [InlineData("Smith", "Smyth")]
    [InlineData("John", "Jon")]
    [InlineData("Catherine", "Katherine")]
    public void Match_PhoneticallySimilar_ReturnsTrue(string source, string target)
    {
        // Act
        var result = _matcher.Match(source, target) as CaverMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsPhoneticallySimilar, 
            $"Expected {source} and {target} to be phonetically similar");
    }

    [Theory]
    [InlineData("Smith", "Jones")]
    [InlineData("Cat", "Dog")]
    public void Match_NotPhoneticallySimilar_ReturnsFalse(string source, string target)
    {
        // Act
        var result = _matcher.Match(source, target) as CaverMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsPhoneticallySimilar,
            $"Expected {source} and {target} to NOT be phonetically similar");
    }

    [Fact]
    public void Match_NullStrings_ReturnsMissingInBoth()
    {
        // Act
        var result = _matcher.Match(null, null) as CaverMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.MissingInSource);
        Assert.True(result.MissingInTarget);
    }
}
