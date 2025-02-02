using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using FightinWords.WordLookup;

namespace FightinWords.Scoring;

/// <summary>
/// Gives you a score based on standard <a href="https://en.wikipedia.org/wiki/Scrabble_letter_distributions">Scrabble letter distributions</a>.
/// </summary>
public sealed class ScrabbleScorer : IScorer
{
    [SuppressMessage("ReSharper", "SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault")]
    public static int GetLetterScore(Rune letter, Language language)
    {
        var lower = Rune.ToLowerInvariant(letter);
        return language switch
        {
            Language.English   => GetEnglishLetterScore(lower),
            Language.German    => GetGermanLetterScore(lower),
            Language.Afrikaans => GetAfrikaansLetterScore(lower),
            _ => throw new ArgumentOutOfRangeException(nameof(language), language,
                "Not a language that plays Scrabble!")
        };
    }

    [SuppressMessage("ReSharper", "SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault")]
    public static int GetLetterCount(Rune letter, Language language)
    {
        var lower = Rune.ToLowerInvariant(letter);
        return language switch
        {
            Language.English   => GetEnglishLetterCount(lower),
            Language.German    => throw new NotImplementedException(),
            Language.Afrikaans => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(language), language,
                "Not a language that plays Scrabble!")
        };
    }

    /// <summary>
    /// <code><![CDATA[
    /// 2 blank tiles (scoring 0 points)
    /// 1 point: E ×12, A ×9, I ×9, O ×8, N ×6, R ×6, T ×6, L ×4, S ×4, U ×4
    /// 2 points: D ×4, G ×3
    /// 3 points: B ×2, C ×2, M ×2, P ×2
    /// 4 points: F ×2, H ×2, V ×2, W ×2, Y ×2
    /// 5 points: K ×1
    /// 8 points: J ×1, X ×1
    /// 10 points: Q ×1, Z ×1
    /// ]]></code>
    /// </summary>
    /// <param name="letter"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static int GetEnglishLetterScore(Rune letter)
    {
        Debug.Assert(!Rune.IsUpper(letter));
        return letter.Value switch
        {
            'a' or 'e' or 'i' or 'l' or 'n' or 'o' or 'r' or 's' or 't' or 'u' => 1,
            'd' or 'g'                                                         => 2,
            'b' or 'c' or 'm' or 'p'                                           => 3,
            'f' or 'h' or 'v' or 'w' or 'y'                                    => 4,
            'k'                                                                => 5,
            'j' or 'x'                                                         => 8,
            'q' or 'z'                                                         => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(letter), letter,
                "Not a valid letter in English Scrabble")
        };
    }

    private static int GetEnglishLetterCount(Rune letter)
    {
        Debug.Assert(!Rune.IsUpper(letter));
        return letter.Value switch
        {
            'e' => 12,
            'a' or 'i' => 9,
            'o' => 8,
            'n' or 'r' or 't' => 6,
            'l' or 's' or 'u' or 'd' => 4,
            'g' => 3,
            'b' or 'c' or 'm' or 'p' or 'f' or 'h' or 'v' or 'w' or 'y' => 2,
            'k' or 'j' or 'x' or 'q' or 'z' => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(letter), letter, "Not a valid letter in English Scrabble")
        };
    }

    /// <summary>
    /// <code><![CDATA[
    /// 2 blank tiles (scoring 0 points)
    /// 1 point: E ×12, A ×9, I ×9, O ×8, N ×6, R ×6, T ×6, L ×4, S ×4, U ×4
    /// 2 points: D ×4, G ×3
    /// 3 points: B ×2, C ×2, M ×2, P ×2
    /// 4 points: F ×2, H ×2, V ×2, W ×2, Y ×2
    /// 5 points: K ×1
    /// 8 points: J ×1, X ×1
    /// 10 points: Q ×1, Z ×1
    /// ]]></code>
    /// </summary>
    private static int GetAfrikaansLetterScore(Rune letter)
    {
        Debug.Assert(!Rune.IsUpper(letter));
        return letter.Value switch
        {
            'e' or 'a' or 'i' or 'o' or 'n' or 'r' or 't' or 'l' or 's' or 'u' => 1,
            'd' or 'g' => 2,
            'b' or 'c' or 'm' or 'p' => 3,
            'f' or 'h' or 'v' or 'w' or 'y' => 4,
            'k' => 5,
            'j' or 'x' => 8,
            'q' or 'z' => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(letter), letter, null)
        };
    }

    /// <summary>
    /// <code><![CDATA[
    /// 2 blank tiles (scoring 0 points)
    /// 1 point: E ×15, N ×9, S ×7, I ×6, R ×6, T ×6, U ×6, A ×5, D ×4
    /// 2 points: H ×4, G ×3, L ×3, O ×3
    /// 3 points: M ×4, B ×2, W ×1, Z ×1
    /// 4 points: C ×2, F ×2, K ×2, P ×1
    /// 6 points: Ä ×1, J ×1, Ü ×1, V ×1
    /// 8 points: Ö ×1, X ×1
    /// 10 points: Q ×1, Y ×1
    /// ]]></code>
    /// </summary>
    /// <param name="letter"></param>
    /// <returns></returns>
    private static int GetGermanLetterScore(Rune letter)
    {
        Debug.Assert(!Rune.IsUpper(letter));
        return letter.Value switch
        {
            'e' or 'n' or 's' or 'i' or 'r' or 't' or 'u' or 'a' or 'd' => 1,
            'h' or 'g' or 'l' or 'o' => 2,
            'm' or 'b' or 'w' or 'z' => 3,
            'c' or 'f' or 'k' or 'p' => 4,
            'ä' or 'j' or 'ü' or 'v' => 6,
            'ö' or 'x' => 8,
            'q' or 'y' => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(letter), letter, null)
        };
    }

    public int ComputeScore(ReadOnlySpan<Rune> word, Language language)
    {
        var sum = 0;

        foreach (var rune in word)
        {
            sum += GetLetterScore(rune, language);
        }

        return sum;
    }

    public int ComputeScore(SpanRuneEnumerator word, Language language)
    {
        var sum = 0;

        foreach (var rune in word)
        {
            sum += GetLetterScore(rune, language);
        }

        return sum;
    }
}