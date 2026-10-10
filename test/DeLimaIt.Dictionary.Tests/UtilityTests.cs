using DeLimaIt.Dictionary.Application.Shared;

namespace DeLimaIt.Dictionary.Tests;

public sealed class UtilityTests
{
    private static readonly Dictionary<string, Dictionary<string, string>> Dictionaries = new()
    {
        ["Colors"] = new Dictionary<string, string> { ["1"] = "Red", ["blue"] = "Blue" }
    };

    [Fact]
    public void String_lookup_returns_value_or_empty_when_dictionary_or_key_is_missing()
    {
        Assert.Equal("Blue", "blue".ToStringFromDictionary("Colors", Dictionaries));
        Assert.Null("missing".ToStringFromDictionary("Colors", Dictionaries));
        Assert.Equal(string.Empty, "blue".ToStringFromDictionary("Unknown", Dictionaries));
    }

    [Fact]
    public void Integer_lookup_uses_invariant_integer_key_and_empty_fallback()
    {
        Assert.Equal("Red", 1.ToStringFromDictionary("Colors", Dictionaries));
        Assert.Null(99.ToStringFromDictionary("Colors", Dictionaries));
        Assert.Equal(string.Empty, 1.ToStringFromDictionary("Unknown", Dictionaries));
    }

    [Fact]
    public void String_lookup_with_default_returns_key_if_dictionary_exists_but_key_does_not()
    {
        Assert.Equal("Blue", "blue".ToStringFromDictionaryOrDefault("Colors", Dictionaries));
        Assert.Equal("missing", "missing".ToStringFromDictionaryOrDefault("Colors", Dictionaries));
        Assert.Equal(string.Empty, "missing".ToStringFromDictionaryOrDefault("Unknown", Dictionaries));
    }

    [Fact]
    public void Decimal_formatters_use_expected_cultures_without_group_separators()
    {
        Assert.Equal("1234,5", 1234.5m.ToBrString());
        Assert.Equal("1234.5", 1234.5m.ToGbString());
        Assert.Equal("0", 0m.ToBrString());
    }
}
