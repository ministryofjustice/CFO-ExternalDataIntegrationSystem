using System.Text.RegularExpressions;

namespace Matching.Core.Utils;

internal static class CaverUtils
{
    private static readonly Regex Alpha = new Regex("[^a-z]", RegexOptions.Compiled);
    private static readonly Regex LowerVowel = new Regex("[aeiou]", RegexOptions.Compiled);

    internal static bool IsPhoneticallySimilar(string? source, string? target)
    {
        if (source is null || target is null)
        {
            return false;
        }

        if (StringUtils.Equal(source, target))
        {
            return true;
        }

        bool similar = IsSimilar([source, target]);
        return similar;
    }

    private static string BuildKey(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        string input = word.ToLower();
        input = Alpha.Replace(input, string.Empty);
        if (input == string.Empty)
        {
            return string.Empty;
        }

        if (input.EndsWith("e", StringComparison.Ordinal))
        {
            input = input.Substring(0, input.Length - 1);
        }

        if (input.Length > 1)
        {

            input = TranslateStartsWith(input);
            input = TranslateRemaining(input);
        }

        input += "1111111111";
        return input.Substring(0, 10);
    }

    private static string TranslateRemaining(string key)
    {
        int length = key.Length;
        key = key.Replace("cq", "2q");
        key = key.Replace("ci", "si");
        key = key.Replace("ce", "se");
        key = key.Replace("cy", "sy");
        key = key.Replace("tch", "2ch");
        key = key.Replace("c", "k");
        key = key.Replace("q", "k");
        key = key.Replace("x", "k");
        key = key.Replace("v", "f");
        key = key.Replace("dg", "2g");
        key = key.Replace("tio", "sio");
        key = key.Replace("tia", "sia");
        key = key.Replace("d", "t");
        key = key.Replace("ph", "fh");
        key = key.Replace("b", "p");
        key = key.Replace("sh", "s2");
        key = key.Replace("z", "s");
        if (LowerVowel.IsMatch(key.Substring(0, 1)))
        {
            key = "A" + key.Substring((1 < length) ? 1 : length);
        }

        key = LowerVowel.Replace(key, "3");
        key = key.Replace("j", "y");
        if (key.StartsWith("y3"))
        {
            key = "Y3" + key.Substring((2 < length) ? 2 : length);
        }

        if (key.Substring(0, 1) == "y")
        {
            key = "A" + key.Substring((1 < length) ? 1 : length);
        }

        key = key.Replace("y", "3");
        key = key.Replace("3gh3", "3kh3");
        key = key.Replace("gh", "22");
        key = key.Replace("g", "k");
        key = Regex.Replace(key, "[s]{1,}", "S");
        key = Regex.Replace(key, "[t]{1,}", "T");
        key = Regex.Replace(key, "[p]{1,}", "P");
        key = Regex.Replace(key, "[k]{1,}", "K");
        key = Regex.Replace(key, "[f]{1,}", "F");
        key = Regex.Replace(key, "[m]{1,}", "M");
        key = Regex.Replace(key, "[n]{1,}", "N");
        key = key.Replace("w3", "W3");
        key = key.Replace("wh3", "Wh3");
        if (key.EndsWith("w", StringComparison.Ordinal))
        {
            key = key.Substring(0, key.Length - 1) + "3";
        }

        key = key.Replace("w", "2");
        if (key.Substring(0, 1) == "h")
        {
            key = "A" + key.Substring((1 < length) ? 1 : length);
        }

        key = key.Replace("h", "2");
        key = key.Replace("r3", "R3");
        if (key.EndsWith("r", StringComparison.Ordinal))
        {
            key = key.Substring(0, key.Length - 1) + "3";
        }

        key = key.Replace("r", "2");
        key = key.Replace("l3", "L3");
        if (key.EndsWith("l", StringComparison.Ordinal))
        {
            key = key.Substring(0, key.Length - 1) + "3";
        }

        key = key.Replace("l", "2");
        key = key.Replace("2", string.Empty);
        if (key.EndsWith("3", StringComparison.Ordinal))
        {
            key = key.Substring(0, key.Length - 1) + "A";
        }

        key = key.Replace("3", string.Empty);
        return key;
    }

    private static string TranslateStartsWith(string name)
    {
        int length = name.Length;
        if (name.StartsWith("cough", StringComparison.Ordinal))
        {
            name = "cou2f" + name.Substring((5 < length) ? 5 : length);
        }
        else if (name.StartsWith("rough", StringComparison.Ordinal))
        {
            name = "rou2f" + name.Substring((5 < length) ? 5 : length);
        }
        else if (name.StartsWith("tough", StringComparison.Ordinal))
        {
            name = "tou2f" + name.Substring((5 < length) ? 5 : length);
        }
        else if (name.StartsWith("trough", StringComparison.Ordinal))
        {
            name = "trou2f" + name.Substring((6 < length) ? 6 : length);
        }
        else if (name.StartsWith("enough", StringComparison.Ordinal))
        {
            name = "enou2f" + name.Substring((6 < length) ? 6 : length);
        }
        else if (name.StartsWith("gn", StringComparison.Ordinal))
        {
            name = "2n" + name.Substring((2 < length) ? 2 : length);
        }
        else if (name.StartsWith("mb", StringComparison.Ordinal))
        {
            name = "m2" + name.Substring((2 < length) ? 2 : length);
        }

        return name;
    }

    private static bool IsSimilar(string[] words)
    {
        string[] array = new string[words.Length];
        for (int i = 0; i < words.Length; i++)
        {
            array[i] = BuildKey(words[i]);
            if (i != 0 && array[i] != array[i - 1])
            {
                return false;
            }
        }

        return true;
    }
}
