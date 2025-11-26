# Matching.Core

This library provides the core functionality for the matching engine. It defines the main interfaces and classes for implementing different matching strategies.

## Core Components

### `IMatcher<T, Result>`

This is the primary interface for implementing a matcher. It has a single method:

```csharp
Result Match(T source, T target);
```

- `T`: The type of the objects to compare.
- `Result`: A type that inherits from `MatcherResult`, representing the result of the match.

### `IMatcher`

This is a non-generic version of the `IMatcher` interface, intended for use when the types are not known at compile time.

### `Matcher<T, Result>`

This abstract class provides a base implementation of `IMatcher`. It handles type checking and casting, and provides an abstract `Match` method for subclasses to implement:

```csharp
protected abstract Result Match(T? source, T? target);
```

### `MatcherResult`

This abstract class represents the result of a matching operation. It contains the source and target objects and properties to indicate if one or both are missing.

## Available Matchers

The following matchers are available in the `Matching.Core.Matchers` namespace:

- `CaverMatcher`: Matches based on the CAVER algorithm.
- `DateMatcher`: Matches dates with a configurable tolerance.
- `EqualityMatcher`: Performs a simple equality check.
- `JaroWinklerMatcher`: Uses the Jaro-Winkler distance algorithm for string comparison.
- `LevenshteinMatcher`: Uses the Levenshtein distance algorithm for string comparison.
- `PostcodeMatcher`: Matches UK postcodes.

## Usage

Here is an example of how to use the `EqualityMatcher`:

```csharp
using Matching.Core.Matchers;

var matcher = new EqualityMatcher();
var result = matcher.Match("hello", "hello");

Console.WriteLine(result.Equal); // True
```

## Search

### `Precedence`

The `Precedence` class is a static class used to calculate a precedence score for a match. This score is used to rank potential matches. It takes in identifiers, last names, and dates of birth, and uses a combination of exact and fuzzy matching to generate a score.

## Attributes

### `MatcherAttribute`

This attribute is used to decorate a matcher class with a key. This key can be used to dynamically discover and register matchers.

```csharp
[Matcher("Equality")]
public class EqualityMatcher : Matcher<string, EqualityMatcherResult>
{
    // ...
}
```

## Utils

The `Matching.Core.Utils` namespace contains various utility classes for string manipulation, date calculations, and implementations of the matching algorithms.


