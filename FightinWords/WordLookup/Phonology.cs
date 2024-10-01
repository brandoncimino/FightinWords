using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;

namespace FightinWords.WordLookup;

public enum Phonology
{
    Vowel,
    Semivowel,
    Consonant,
    Unknown,
}

public static class PhonologyExtensions
{
    private static readonly FrozenSet<string> EnglishVowels = ((string[])["a", "e", "i", "o", "u", "y"]).ToFrozenSet(StringComparer.Create(CultureInfo.InvariantCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase));

    public static Phonology GetPhonology(this Grapheme grapheme, Language language)
    {
        return language switch
        {
            Language.English or Language.German => GetEnglishPhonology(grapheme),
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
    }

    private static Phonology GetEnglishPhonology(Grapheme grapheme)
    {
        if (EnglishVowels.TryGetValue(grapheme.Source, out var vowel))
        {
            return vowel is "y" ? Phonology.Semivowel : Phonology.Vowel;
        }

        return Phonology.Consonant;
    }
}