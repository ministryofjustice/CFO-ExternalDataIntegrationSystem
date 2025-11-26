using Matching.Core.Search;

namespace Api.Tests;

public class CandidateTests
{
    [Theory]
    [InlineData("A12345", "A12345", "John", "John", "1999-10-11", "1999-10-11", 1)]      // Identical, Identical, Identical
    [InlineData("A12345", "A12345", "John", "Johnny", "1999-10-11", "1999-10-11", 2)]    // Identical, Similar, Identical
    [InlineData("A12345", "A12345", "John", "John", "1999-10-11", "1999-11-10", 3)]      // Identical, Identical, Similar
    [InlineData("A12345", "A12354", "John", "John", "1999-10-11", "1999-10-11", 4)]      // Similar, Identical, Identical
    [InlineData("A12345", "A12345", "John", "Dave", "1999-10-11", "1999-10-11", 5)]      // Identical, Different, Identical
    [InlineData("A12345", "A12345", "John", "Johnny", "1999-10-11", "1999-11-10", 6)]    // Identical, Similar, Similar
    [InlineData("A12345", "A12345", "John", "John", "2000-01-12", "1999-10-11", 7)]      // Identical, Identical, Different
    [InlineData("A12345", "A12345", "John", "Dave", "1999-10-11", "1999-11-10", 8)]      // Identical, Different, Identical
    [InlineData("A12345", "A12345", "John", "Johnny", "2000-01-12", "1999-10-11", 9)]    // Identical, Similar, Different
    [InlineData("A12345", "A56789", "John", "John", "1999-10-11", "1999-10-11", 10)]     // Different, Identical, Identical
    [InlineData("A12345", "A12345", "John", "Dave", "2000-01-12", "1999-10-11", 11)]     // Identical, Different, Different
    public void SearchTest(string leftIdentifier, string rightIdentifier, string leftName, string rightName, string leftDate, string rightData, int expectedPrecedence)
    {
        var actualPrecedence = Precedence.GetPrecedence((leftIdentifier, rightIdentifier), (leftName, rightName), (DateOnly.Parse(leftDate), DateOnly.Parse(rightData)));
        Assert.Equal(actualPrecedence, expectedPrecedence);
    }

}
